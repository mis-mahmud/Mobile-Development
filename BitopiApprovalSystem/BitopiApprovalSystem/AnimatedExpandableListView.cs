using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Util;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Views.Animations;
using Java.Lang;
using Android.Animation;

using System.Threading.Tasks;
using BitopiApprovalSystem.Library;

namespace BitopiApprovalSystem.Widget
{
    public class AnimatedExpandableListView : PullToRefresharp.Android.Widget.ExpandableListView
    {
        //static readonly string TAG=AnimatedExapndableListAdapter.class.getSimpleName();
        static readonly int ANIMATION_DURATION = 500;
        public AnimatedExpandableListAdapter adapter;
        public Activity activity { get; set; }

        public Action OnAnimationEnd;
        public AnimatedExpandableListView(Context context)
            : base(context)
        {

        }
        public AnimatedExpandableListView(Context context, Android.Util.IAttributeSet attrs)
            : base(context, attrs)
        {

        }
        public AnimatedExpandableListView(Context context, Android.Util.IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {

        }
        public override void SetAdapter(IExpandableListAdapter adapter)
        {
            base.SetAdapter(adapter);
            if (adapter is AnimatedExpandableListAdapter)
            {
                this.adapter = (AnimatedExpandableListAdapter)adapter;
                this.adapter.setParent(this);
            }
            else
            {

            }
        }

        public void SetContent()
        {
            //this.SetAdapter(new AnimatedExpandableListAdapter(activity));
            //this.adapter.TruckList = trucks;
            //this.adapter.NotifyDataSetChanged();
        }
        int lastExpandedGroupPosition = 0;
        public bool expandGroupWithAnimation(int groupPos, Action onAnimationEnd = null, bool lastGroupAnimated = false)
        {
            this.OnAnimationEnd = onAnimationEnd;
            //if (onAnimationEnd != null)
            //{
            //    Task.Delay(100).ContinueWith(t=>onAnimationEnd());
            //}
            bool lastGroup = groupPos == adapter.GroupCount - 1;
            if (lastGroup && Build.VERSION.SdkInt >= Build.VERSION_CODES.IceCreamSandwich && !lastGroupAnimated)
            {
                if (onAnimationEnd != null)
                {
                    Task.Delay(500).ContinueWith(t => onAnimationEnd());
                }
                return ExpandGroup(groupPos, true);
            }
            int groupFlatPos = GetFlatListPosition(GetPackedPositionForGroup(groupPos));
            if (groupFlatPos != -1)
            {
                int childIndex = groupFlatPos - FirstVisiblePosition;
                if (childIndex < ChildCount)
                {
                    // Get the view for the group is it is on screen...
                    View v = GetChildAt(childIndex);
                    if (v.Bottom >= Bottom)
                    {
                        // If the user is not going to be able to see the animation
                        // we just expand the group without an animation.
                        // This resolves the case where getChildView will not be
                        // called if the children of the group is not on screen

                        // We need to notify the adapter that the group was expanded
                        // without it's knowledge
                        if (onAnimationEnd != null)
                        {
                            Task.Delay(100).ContinueWith(t => onAnimationEnd());
                        }
                        adapter.notifyGroupExpanded(groupPos);
                        return ExpandGroup(groupPos);
                    }
                }
            }

            // Let the adapter know that we are starting the animation...
            adapter.startExpandAnimation(groupPos, 0);
            // readonlyly call expandGroup (note that expandGroup will call
            // notifyDataSetChanged so we don't need to)
            return ExpandGroup(groupPos);

        }
        public bool collapseGroupWithAnimation(int groupPos, Action onAnimationEnd)
        {
            this.OnAnimationEnd = onAnimationEnd;
            int groupFlatPos = GetFlatListPosition(GetPackedPositionForGroup(groupPos));
            if (groupFlatPos != -1)
            {
                int childIndex = groupFlatPos - FirstVisiblePosition;
                if (childIndex >= 0 && childIndex < ChildCount)
                {
                    // Get the view for the group is it is on screen...
                    View v = GetChildAt(childIndex);
                    if (v.Bottom >= Bottom)
                    {
                        // If the user is not going to be able to see the animation
                        // we just collapse the group without an animation.
                        // This resolves the case where getChildView will not be
                        // called if the children of the group is not on screen
                        return CollapseGroup(groupPos);
                    }
                }
                else
                {
                    // If the group is offscreen, we can just collapse it without an
                    // animation...
                    return CollapseGroup(groupPos);
                }
            }

            // Get the position of the firstChild visible from the top of the screen
            long packedPos = GetExpandableListPosition(FirstVisiblePosition);
            int firstChildPos = GetPackedPositionChild(packedPos);
            int firstGroupPos = GetPackedPositionGroup(packedPos);

            // If the first visible view on the screen is a child view AND it's a
            // child of the group we are trying to collapse, then set that
            // as the first child position of the group... see
            // {@link #startCollapseAnimation(int, int)} for why this is necessary
            firstChildPos = firstChildPos == -1 || firstGroupPos != groupPos ? 0 : firstChildPos;

            // Let the adapter know that we are going to start animating the
            // collapse animation.
            adapter.startCollapseAnimation(groupPos, firstChildPos);

            // Force the listview to refresh it's views
            adapter.NotifyDataSetChanged();
            return IsGroupExpanded(groupPos);
        }

        private int getAnimationDuration()
        {
            return ANIMATION_DURATION;
        }

        /**
         * Used for holding information regarding the group.
         */


        /**
         * A specialized adapter for use with the AnimatedExpandableListView. All
         * adapters used with AnimatedExpandableListView MUST extend this class.
         */

        public class AnimatedExpandableListAdapter : BaseExpandableListAdapter, IExpandableListAdapter
        {
            private SparseArray<GroupInfo> groupInfo = new SparseArray<GroupInfo>();
            private AnimatedExpandableListView parent;
            int factor = 0;
            private static readonly Number STATE_IDLE = (Number)0;
            private static readonly Number STATE_EXPANDING = (Number)1;
            private static readonly Number STATE_COLLAPSING = (Number)2;
            public object animationCount = 0;

            Activity _activity;
            Filter filter;

            int currentSelectedItem;

            public AnimatedExpandableListAdapter()
            {

            }


            public AnimatedExpandableListAdapter(Activity activity)
            {
                _activity = activity;

            }

            public void setParent(AnimatedExpandableListView parent)
            {
                this.parent = parent;
            }

            public int getRealChildType(int groupPosition, int childPosition)
            {
                return 0;
            }


            public int getRealChildTypeCount()
            {
                return 1;
            }

            public virtual View getRealChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
            {
                View row = convertView;
                if (row == null)
                {
                    //row = _activity.LayoutInflater.Inflate(Resource.Layout.grid_expanded, null);
                }
                // var TVlstFiveLoc = row.FindViewById<TextView>(Resource.Id.TVlstFiveLoc);
                currentSelectedItem = groupPosition;
                //TVlstFiveLoc.Click -= ShowMapModalEvent;
                //TVlstFiveLoc.Click += ShowMapModalEvent;
                return row;
            }
            public override int GroupCount
            {
                get
                {
                    return 0;
                }
            }
            public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
            {
                return null;
            }
            //void ShowMapModalEvent(object sender, EventArgs e)
            //{
            //    ((TruckListActivity)_activity).ShowMapModal(currentSelectedItem);
            //}
            public virtual int getRealChildrenCount(int groupPosition)
            {
                return 1;
            }

            private GroupInfo getGroupInfo(int groupPosition)
            {
                GroupInfo info = groupInfo.Get(groupPosition);
                if (info == null)
                {
                    info = new GroupInfo();
                    groupInfo.Put(groupPosition, info);
                }

                return info;
            }

            public void notifyGroupExpanded(int groupPosition)
            {
                GroupInfo info = getGroupInfo(groupPosition);
                info.dummyHeight = -1;
            }

            public void startExpandAnimation(int groupPosition, int firstChildPosition)
            {
                GroupInfo info = getGroupInfo(groupPosition);
                info.animating = true;
                info.firstChildPosition = firstChildPosition;
                info.expanding = true;
                info.ChildViewCallCount = 0;
            }

            public void startCollapseAnimation(int groupPosition, int firstChildPosition)
            {
                GroupInfo info = getGroupInfo(groupPosition);
                info.animating = true;
                info.firstChildPosition = firstChildPosition;
                info.expanding = false;
            }

            private void stopAnimation(int groupPosition)
            {
                GroupInfo info = getGroupInfo(groupPosition);
                info.animating = false;
                info.ChildViewCallCount = 0;

            }

            /**
             * Override {@link #getRealChildType(int, int)} instead.
             */

            public override int GetChildType(int groupPosition, int childPosition)
            {
                GroupInfo info = getGroupInfo(groupPosition);
                if (info.animating)
                {
                    // If we are animating this group, then all of it's children
                    // are going to be dummy views which we will say is type 0.
                    return 0;
                }
                else
                {
                    // If we are not animating this group, then we will add 1 to
                    // the type it has so that no type id conflicts will occur
                    // unless getRealChildType() returns MAX_INT
                    return getRealChildType(groupPosition, childPosition) + 1;
                }
            }

            /**
             * Override {@link #getRealChildTypeCount()} instead.
             */

            public override int ChildTypeCount
            {
                get
                {
                    return getRealChildTypeCount() + 1;
                }
            }
            protected ViewGroup.LayoutParams generateDefaultLayoutParams()
            {
                return new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                                                    ViewGroup.LayoutParams.WrapContent, 0);
            }

            /**
             * Override {@link #getChildView(int, int, bool, View, ViewGroup)} instead.
             */


            public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
            {
                try
                {
                    GroupInfo info = getGroupInfo(groupPosition);
                    info.ChildViewCallCount += 1; ;
                    if (info.animating)
                    {
                        // If this group is animating, return the a DummyView...
                        if (convertView is DummyView == false)
                        {
                            convertView = new DummyView(parent.Context);
                            convertView.LayoutParameters = new AbsListView.LayoutParams(LayoutParams.MatchParent, 0);

                        }

                        if (childPosition < info.firstChildPosition)
                        {
                            // The reason why we do this is to support the collapse
                            // this group when the group view is not visible but the
                            // children of this group are. When notifyDataSetChanged
                            // is called, the ExpandableListView tries to keep the
                            // list position the same by saving the first visible item
                            // and jumping back to that item after the views have been
                            // refreshed. Now the problem is, if a group has 2 items
                            // and the first visible item is the 2nd child of the group
                            // and this group is collapsed, then the dummy view will be
                            // used for the group. But now the group only has 1 item
                            // which is the dummy view, thus when the ListView is trying
                            // to restore the scroll position, it will try to jump to
                            // the second item of the group. But this group no longer
                            // has a second item, so it is forced to jump to the next
                            // group. This will cause a very ugly visual glitch. So
                            // the way that we counteract this is by creating as many
                            // dummy views as we need to maintain the scroll position
                            // of the ListView after notifyDataSetChanged has been
                            // called.
                            convertView.LayoutParameters.Height = 0;
                            return convertView;
                        }

                        AnimatedExpandableListView listView = (AnimatedExpandableListView)parent;

                        DummyView dummyView = (DummyView)convertView;

                        // Clear the views that the dummy view draws.
                        dummyView.clearViews();

                        // Set the style of the divider
                        dummyView.setDivider(listView.Divider, parent.MeasuredWidth, listView.DividerHeight);

                        // Make measure specs to measure child views
                        int measureSpecW = MeasureSpec.MakeMeasureSpec(parent.Width, MeasureSpecMode.Exactly);
                        int measureSpecH = MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);

                        int totalHeight = 0;
                        int clipHeight = parent.Height;

                        int len = getRealChildrenCount(groupPosition);
                        for (int i = info.firstChildPosition; i < len; i++)
                        {
                            View childView = getRealChildView(groupPosition, i, (i == len - 1), null, parent);

                            LayoutParams p = (LayoutParams)childView.LayoutParameters;
                            if (p == null)
                            {
                                p = (AbsListView.LayoutParams)generateDefaultLayoutParams();
                                childView.LayoutParameters = p;
                            }

                            int lpHeight = p.Height;

                            int childHeightSpec;
                            if (lpHeight > 0)
                            {
                                childHeightSpec = MeasureSpec.MakeMeasureSpec(lpHeight, MeasureSpecMode.Exactly);
                            }
                            else
                            {
                                childHeightSpec = measureSpecH;
                            }

                            childView.Measure(measureSpecW, childHeightSpec);
                            totalHeight += childView.MeasuredHeight;

                            if (totalHeight < clipHeight)
                            {
                                // we only need to draw enough views to fool the user...
                                dummyView.addFakeView(childView);
                            }
                            else
                            {
                                dummyView.addFakeView(childView);

                                // if this group has too many views, we don't want to
                                // calculate the height of everything... just do a light
                                // approximation and break
                                int averageHeight = totalHeight / (i + 1);
                                totalHeight += (len - i - 1) * averageHeight;
                                break;
                            }
                        }

                        object o = dummyView.Tag;


                        Java.Lang.Number state = o == null ? STATE_IDLE : (Java.Lang.Number)o;

                        if (info.expanding && state != STATE_EXPANDING && info.ChildViewCallCount <= 1) //&& info.ChildViewCallCount<=1
                        {
                            //new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                            //{
                            //  _activity.RunOnUiThread(() =>
                            //{
                            ExpandAnimation ani = new ExpandAnimation(dummyView, 0, totalHeight, info, _activity, this);

                            ani.Duration = this.parent.getAnimationDuration();
                            //ani.SetInterpolator(_activity, Android.Resource.Animation.LinearInterpolator);
                            ani.Interpolator = new AccelerateDecelerateInterpolator();
                            ani.SetAnimationListener(new CustomAnimateListener(STATE_EXPANDING, groupPosition, dummyView, info, listView,
                                this, listView.OnAnimationEnd));
                            //ani.FillAfter = false;
                            dummyView.StartAnimation(ani);
                            //ani.RepeatCount = Animation.StartOnFirstFrame;


                            /*****************Another Animation Implementation*****************
                             ValueAnimator animator = ValueAnimator.OfInt(0, totalHeight);
                                  animator.SetDuration(this.parent.getAnimationDuration());
                                  animator.SetInterpolator(new AccelerateDecelerateInterpolator());

                                  animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
                                  {


                                      var value = (int)animator.AnimatedValue;
                                      if (factor != value)
                                      {
                                          factor = value;
                                          ViewGroup.LayoutParams layoutParams = dummyView.LayoutParameters;
                                          layoutParams.Height = value;
                                          dummyView.LayoutParameters = layoutParams;
                                          Console.System.Diagnostics.Debug.WriteLine("Value: " + value.ToString());
                                      }

                                  };
                             * animator.AddListener(new CustomAnimateListener(STATE_EXPANDING, groupPosition, dummyView, info, listView, this));
                             * * ************************/




                            dummyView.Tag = STATE_EXPANDING;
                            // });
                            //})).Start();
                        }
                        else if (!info.expanding && state != STATE_COLLAPSING && info.ChildViewCallCount <= 1)  //&& info.ChildViewCallCount<=1
                        {
                            if (info.dummyHeight == -1)
                            {
                                info.dummyHeight = totalHeight;
                            }
                            //new System.Threading.Thread(new System.Threading.ThreadStart(delegate
                            //{
                            //  _activity.RunOnUiThread(() =>
                            //{
                            ExpandAnimation ani = new ExpandAnimation(dummyView, info.dummyHeight, 0, info, _activity, this);
                            ani.Duration = this.parent.getAnimationDuration();
                            //ani.SetInterpolator(_activity, Android.Resource.Animation.AccelerateInterpolator);
                            ani.Interpolator = new AccelerateDecelerateInterpolator();
                            ani.SetAnimationListener(new CustomAnimateListener(STATE_COLLAPSING, groupPosition, dummyView, info, listView,
                                this, listView.OnAnimationEnd));
                            long i = ani.ComputeDurationHint();
                            dummyView.StartAnimation(ani);

                            /**********************************Another animation implementation********************
                              ValueAnimator animator = ValueAnimator.OfInt(totalHeight, 0);
                                                       animator.SetDuration(this.parent.getAnimationDuration());
                                                       animator.SetInterpolator(new AccelerateDecelerateInterpolator());
                                                       animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
                                                       {
                                                           var value = (int)animator.AnimatedValue;
                                                           if (factor != value)
                                                           {
                                                               factor = value;
                                                               ViewGroup.LayoutParams layoutParams = dummyView.LayoutParameters;
                                                               layoutParams.Height = value;
                                                               dummyView.LayoutParameters = layoutParams;
                                                               Console.System.Diagnostics.Debug.WriteLine("Value: " + value.ToString());
                                                           }
                                                       };
                                                       animator.AddListener(new CustomAnimateListener(STATE_COLLAPSING,groupPosition,dummyView,info,listView,this));
                             animator.Start();
                             * ****************************************/

                            dummyView.Tag = STATE_COLLAPSING;
                            // });
                            //})).Start();
                        }

                        return convertView;
                    }
                    else
                    {
                        info.ChildViewCallCount = 0;
                        return getRealChildView(groupPosition, childPosition, isLastChild, convertView, parent);
                    }
                }
                catch (System.Exception ex)
                {
                    string s = ex.Message;

                    CustomLogger.CustomLog("From Activity: " + BitopiSingelton.Instance.CurrentActivity + "\nMessage: " + ex.Message + "\nStack Trace: " + ex.StackTrace + "\n\n", "", BitopiSingelton.Instance.User != null ?
                         BitopiSingelton.Instance.User.UserName : "");
                    return null;
                }

            }
            ValueAnimator slideAnimator(View mView, int start, int end)
            {
                ValueAnimator animator = ValueAnimator.OfInt(start, end);
                animator.AddUpdateListener(new CustomUpdateListener(mView));
                return animator;
            }
            #region implemented abstract members of BaseExpandableListAdapter

            public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
            {
                return null;
            }

            public override long GetChildId(int groupPosition, int childPosition)
            {
                return childPosition;
            }

            public override Java.Lang.Object GetGroup(int groupPosition)
            {
                return null;
            }

            //protected override void Javareadonlyize()
            //{
            //    base.Javareadonlyize();
            //}
            public override long GetGroupId(int groupPosition)
            {
                return groupPosition;
            }

            public override bool IsChildSelectable(int groupPosition, int childPosition)
            {
                return true;
            }

            public override bool HasStableIds
            {
                get
                {
                    return true;
                }
            }

            #endregion
            public Filter Filter
            {
                get
                {
                    return filter;
                }
            }

            public override int GetChildrenCount(int groupPosition)
            {
                GroupInfo info = getGroupInfo(groupPosition);
                if (info.animating)
                {
                    return info.firstChildPosition + 1;
                }
                else
                {
                    return getRealChildrenCount(groupPosition);
                }


            }

            private class DummyView : View
            {
                private List<View> views = new List<View>();
                private Drawable divider;
                private int dividerWidth;
                private int dividerHeight;

                public DummyView(Context context)
                    : base(context)
                {

                }

                public void setDivider(Drawable divider, int dividerWidth, int dividerHeight)
                {
                    if (divider != null)
                    {
                        this.divider = divider;
                        this.dividerWidth = dividerWidth;
                        this.dividerHeight = dividerHeight;

                        divider.SetBounds(0, 0, dividerWidth, dividerHeight);
                    }
                }

                /**
                 * Add a view for the DummyView to draw.
                 * @param childView View to draw
                 */
                public void addFakeView(View childView)
                {
                    childView.Layout(0, 0, Width, childView.MeasuredHeight);
                    views.Add(childView);
                }


                protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
                {
                    base.OnLayout(changed, left, top, right, bottom);
                    int len = views.Count();
                    for (int i = 0; i < len; i++)
                    {
                        View v = views.ElementAt(i);
                        v.Layout(left, top, left + v.MeasuredWidth, top + v.MeasuredHeight);
                    }
                }

                public void clearViews()
                {
                    views.Clear();
                }


                public override void Draw(Canvas canvas)
                {
                    canvas.Save();
                    if (divider != null)
                    {
                        divider.SetBounds(0, 0, dividerWidth, dividerHeight);
                    }

                    int len = views.Count();
                    for (int i = 0; i < len; i++)
                    {
                        View v = views.ElementAt(i);

                        canvas.Save();
                        canvas.ClipRect(0, 0, Width, v.MeasuredHeight);
                        v.Draw(canvas);
                        canvas.Restore();

                        if (divider != null)
                        {
                            divider.Draw(canvas);
                            canvas.Translate(0, dividerHeight);
                        }

                        canvas.Translate(0, v.MeasuredHeight);
                    }

                    canvas.Restore();
                }


            }

            class CustomUpdateListener : Java.Lang.Object, ValueAnimator.IAnimatorUpdateListener
            {
                public View _mView;
                public CustomUpdateListener(View mView)
                {
                    _mView = mView;
                }
                public void OnAnimationUpdate(ValueAnimator animation)
                {

                    int value = Convert.ToInt32(animation.AnimatedValue);
                    if (value != 0)
                    {
                        ViewGroup.LayoutParams layoutParams = _mView.LayoutParameters;
                        layoutParams.Height = value;
                        _mView.LayoutParameters = layoutParams;
                    }
                }
            }
            public class CustomAnimateListener : Java.Lang.Object, Animation.IAnimationListener, Android.Animation.Animator.IAnimatorListener
            {
                public void OnAnimationCancel(Animator animation)
                {
                    //throw new NotImplementedException ();
                }

                public void OnAnimationEnd(Animator animation)
                {
                    //try
                    //{
                    if (_state == STATE_EXPANDING)
                    {
                        _parent.stopAnimation(_groupPostion);
                        //_parent.NotifyDataSetChanged();
                        _dummyView.Tag = STATE_IDLE;
                    }
                    if (_state == STATE_COLLAPSING)
                    {
                        _parent.stopAnimation(_groupPostion);
                        _listView.CollapseGroup(_groupPostion);
                        //_parent.NotifyDataSetChanged();
                        _info.dummyHeight = -1;
                        _dummyView.Tag = STATE_IDLE;
                    }
                    _dummyView.SetLayerType(layertype, null);
                    _dummyView.RequestLayout();
                    //}
                    //catch (System.Exception ex)
                    //{
                    //    string s = ex.Message;
                    //}
                    _parent.factor = 0;
                    _parent.animationCount = 0;
                    //if (_listView.OnAnimationEnd != null)
                    //    _listView.OnAnimationEnd();
                    if (OnListViewAnimationEnd != null)
                    {
                        OnListViewAnimationEnd();
                    }
                }

                public void OnAnimationRepeat(Animator animation)
                {
                    //throw new NotImplementedException ();
                }

                public void OnAnimationStart(Animator animation)
                {
                    //throw new NotImplementedException ();
                    layertype = _dummyView.LayerType;
                    _dummyView.SetLayerType(LayerType.Hardware, null);
                    // _parent.IsAnimating = true;
                }
                LayerType layertype;
                Number _state;
                int _groupPostion;
                View _dummyView;
                GroupInfo _info;
                AnimatedExpandableListView _listView;
                AnimatedExpandableListAdapter _parent;
                Action OnListViewAnimationEnd;
                public CustomAnimateListener(Number State, int groupPosition, View dumyView, GroupInfo info, AnimatedExpandableListView listView,
                    AnimatedExpandableListAdapter parent, Action onListViewAnimationEnd)
                {
                    this._state = State;
                    this._groupPostion = groupPosition;
                    this._dummyView = dumyView;
                    _listView = listView;
                    _parent = parent;
                    _info = info;
                    OnListViewAnimationEnd = onListViewAnimationEnd;
                }

                public void OnAnimationEnd(Animation animation)
                {
                    //try
                    //{
                    if (_state == STATE_EXPANDING)
                    {
                        _parent.stopAnimation(_groupPostion);
                        _parent.NotifyDataSetChanged();
                        _dummyView.Tag = STATE_IDLE;
                    }
                    if (_state == STATE_COLLAPSING)
                    {
                        _parent.stopAnimation(_groupPostion);
                        _listView.CollapseGroup(_groupPostion);
                        _parent.NotifyDataSetChanged();
                        _info.dummyHeight = -1;
                        _dummyView.Tag = STATE_IDLE;
                    }
                    _dummyView.SetLayerType(layertype, null);
                    _dummyView.RequestLayout();
                    //}
                    //catch (System.Exception ex)
                    //{
                    //    string s = ex.Message;
                    //}

                    //if (_listView.OnAnimationEnd != null)
                    //  _listView.OnAnimationEnd();
                    if (OnListViewAnimationEnd != null)
                    {
                        OnListViewAnimationEnd();
                    }
                }

                public void OnAnimationRepeat(Animation animation)
                {
                    //throw new NotImplementedException();
                }

                public void OnAnimationStart(Animation animation)
                {
                    //throw new NotImplementedException();
                }


            }

            private class ExpandAnimation : Animation
            {
                private int baseHeight;
                private int delta;
                private View view;
                private GroupInfo groupInfo;
                double customintInterpolatedTime = 0.1;
                float factor = -1;
                Activity _activity;
                AnimatedExpandableListAdapter _parent;
                public ExpandAnimation(View v, int startHeight, int endHeight, GroupInfo info, Activity activity, AnimatedExpandableListAdapter parent)
                {
                    baseHeight = startHeight;
                    delta = endHeight - startHeight;
                    view = v;
                    groupInfo = info;

                    view.LayoutParameters.Height = startHeight;
                    view.RequestLayout();
                    _activity = activity;
                    _parent = parent;
                }


                protected override void ApplyTransformation(float interpolatedTime, Transformation t)
                {
                    base.ApplyTransformation(interpolatedTime, t);
                    if (interpolatedTime < 1.0f)
                    //{
                    //double intInterpolatedTime=System.Math.Round(interpolatedTime,1);
                    //if (interpolatedTime < 1)
                    {

                        double intInterpolatedTime = System.Math.Round(interpolatedTime, 1);

                        int val = baseHeight + (int)(delta * interpolatedTime);
                        _activity.RunOnUiThread(delegate
                        {
                            view.LayoutParameters.Height = val;
                            groupInfo.dummyHeight = val;
                            //view.RequestLayout();
                        }); ;
                    }
                    //}
                    else
                    {
                        int val = baseHeight + delta;
                        _activity.RunOnUiThread(delegate
                        {
                            view.LayoutParameters.Height = val;
                            groupInfo.dummyHeight = val;
                            //view.RequestLayout();
                        });
                    }
                    _activity.RunOnUiThread(() =>
                    {
                        view.RequestLayout();
                        // _parent.NotifyDataSetChanged();
                    });
                }

            }
        }

    }

    public class GroupInfo
    {
        public bool animating = false;
        public bool expanding = false;
        public int firstChildPosition;
        public int ChildViewCallCount = 0;

        /**
         * This variable contains the last known height value of the dummy view.
         * We save this information so that if the user collapses a group
         * before it fully expands, the collapse animation will start from the
         * CURRENT height of the dummy view and not from the full expanded
         * height.
         */
        public int dummyHeight = -1;
    }
    public class HeightAnimation : Animation
    {
        protected int originalHeight;
        protected View view;
        protected float perValue;
        public HeightAnimation(View view, int fromHeight, int toHeight)
        {
            this.view = view;
            this.originalHeight = fromHeight;
            this.perValue = (toHeight - fromHeight);
        }
        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            view.LayoutParameters.Height = (int)(originalHeight + perValue * interpolatedTime);
            view.RequestLayout();
        }
    }
    public class WidthAnimation : Animation
    {
        protected int originalWidth;
        protected View view;
        protected float perValue;
        public WidthAnimation(View view, int fromWidth, int toWidth)
        {
            this.view = view;
            this.originalWidth = fromWidth;
            this.perValue = (toWidth - fromWidth);
        }
        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            view.LayoutParameters.Width = (int)(originalWidth + perValue * interpolatedTime);
            view.RequestLayout();
        }
    }
}