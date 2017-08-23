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
using Android.Animation;
using Java.Lang;
using Android.Views.Animations;
using static Android.Views.Animations.Animation;
using static Android.Views.ViewTreeObserver;

namespace BitopiApprovalSystem
{
    public class DatePickerFragment : DialogFragment,
                                  DatePickerDialog.IOnDateSetListener
    {
        // TAG can be any string of your choice.
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();

        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> _dateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            DatePickerFragment frag = new DatePickerFragment();
            frag._dateSelectedHandler = onDateSelected;
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = DateTime.Now;
            DatePickerDialog dialog = new DatePickerDialog(Activity,
                                                           this,
                                                           currently.Year,
                                                           currently.Month,
                                                           currently.Day);
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);

            _dateSelectedHandler(selectedDate);
        }
    }


    public class CustomAnimatorListenerAdapter : Java.Lang.Object, Animator.IAnimatorListener
    {
        Runnable runnable;
        public CustomAnimatorListenerAdapter(Runnable runnable)
        {
            this.runnable = runnable;
        }
        public void OnAnimationEnd(Animator animation)
        {
            runnable.Run();
        }

        void Animator.IAnimatorListener.OnAnimationCancel(Animator animation)
        {

        }

        void Animator.IAnimatorListener.OnAnimationEnd(Animator animation)
        {

        }

        void Animator.IAnimatorListener.OnAnimationRepeat(Animator animation)
        {

        }

        void Animator.IAnimatorListener.OnAnimationStart(Animator animation)
        {
            throw new NotImplementedException();
        }
    }
    public class CustomAnimatonListenerAdapter : Java.Lang.Object, Animation.IAnimationListener
    {
        Runnable runnable;
        public CustomAnimatonListenerAdapter(Runnable runnable)
        {
            this.runnable = runnable;
        }

        public void OnAnimationEnd(Animation animation)
        {
            throw new NotImplementedException();
        }

        public void OnAnimationEnd(Animator animation)
        {
            runnable.Run();
        }

        public void OnAnimationRepeat(Animation animation)
        {
            throw new NotImplementedException();
        }

        public void OnAnimationStart(Animation animation)
        {
            throw new NotImplementedException();
        }

        void OnAnimationCancel(Animator animation)
        {

        }



        void OnAnimationRepeat(Animator animation)
        {

        }

        void OnAnimationStart(Animator animation)
        {

        }
    }
    public class CustomRunnable : Java.Lang.Object, Java.Lang.IRunnable
    {
        public void Run()
        {

        }
    }
    public class CustomOnPreDrawListener : Java.Lang.Object, IOnPreDrawListener
    {
        ListView listview;
        ViewTreeObserver observer;
        BaseExpandableListAdapter adapter;
        Dictionary<long, int> mItemIdTopMap;
        Action<View, float, float, float, float, Runnable> moveView;
        public CustomOnPreDrawListener(ListView listView, ViewTreeObserver observer, BaseExpandableListAdapter adapter, Dictionary<long, int> mItemIdTopMap,
            Action<View, float, float, float, float, Runnable> moveView)
        {
            this.listview = listView;
            this.observer = observer;
            this.adapter = adapter;
            this.mItemIdTopMap = mItemIdTopMap;
            this.moveView = moveView;
        }

        public bool OnPreDraw()
        {
            observer.RemoveOnPreDrawListener(this);
            bool firstAnimation = true;

            int firstVisiblePosition = listview.FirstVisiblePosition;
            for (int i = 0; i < listview.ChildCount; ++i)
            {
                View child = listview.GetChildAt(i);
                int position = firstVisiblePosition + i;

                long itemId = adapter.GetGroupId(position);
                int startTop = -1;
                if (mItemIdTopMap.ContainsKey(itemId))
                    startTop=mItemIdTopMap[itemId];
                int top = child.Top;
                if (startTop == -1)
                {
                    // Animate new views along with the others. The catch is that they did not
                    // exist in the start state, so we must calculate their starting position
                    // based on whether they're coming in from the bottom (i > 0) or top.
                    int childHeight = child.Height + listview.DividerHeight;
                    startTop = top + (i > 0 ? childHeight : -childHeight);
                }
                int delta = startTop - top;//listView.setEnabled(true);
                if (delta != 0)
                {
                    Runnable endAction = firstAnimation ?
                            new Runnable(() =>
                            {
                                listview.Enabled = (true);
                            }) : null;
                    firstAnimation = false;
                    moveView(child, 0, 0, delta, 0, endAction);
                }
            }
            mItemIdTopMap.Clear();
            return true;
        }

    }
    public class ListViewAnimationHelper
    {

        BaseExpandableListAdapter adapter;
        ListView listView;
        MyTaskList dataSource;
        Dictionary<long, int> mItemIdTopMap = new Dictionary<long, int>();
        private static int MOVE_DURATION = 150;

        // ================================================================================ 
        // Constructor
        // ================================================================================ 

        public ListViewAnimationHelper(BaseExpandableListAdapter adapter, ListView listView, MyTaskList dataSource)
        {
            this.adapter = adapter;
            this.listView = listView;
            this.dataSource = dataSource;
        }
        public ListViewAnimationHelper(BaseExpandableListAdapter adapter, ListView listView)
        {
            this.adapter = adapter;
            this.listView = listView;

        }

        // ================================================================================ 
        // ListView row animation helper
        // ================================================================================ 

        /**
         * This method animates all other views in the ListView container (not including ignoreView)
         * into their final positions. It is called after ignoreView has been removed from the
         * adapter, but before layout has been run. The approach here is to figure out where
         * everything is now, then allow layout to run, then figure out where everything is after
         * layout, and then to run animations between all of those start/end positions.
         */

        public void animateRemoval(ListView listview, View viewToRemove)
        {
            int firstVisiblePosition = listview.FirstVisiblePosition;
            for (int i = 0; i < listview.ChildCount; ++i)
            {
                View child = listview.GetChildAt(i);
                if (child != viewToRemove)
                {
                    int pos = firstVisiblePosition + i;
                    long itemId = adapter.GetGroupId(pos);
                    if (!mItemIdTopMap.ContainsKey(itemId))
                        mItemIdTopMap.Add(itemId, child.Top);
                    else
                        mItemIdTopMap[itemId] = child.Top;
                }
            }
            // Delete the item from the adapter 
            int position = listView.GetPositionForView(viewToRemove);
            dataSource.RemoveAt(position);
            adapter.NotifyDataSetChanged();

            ViewTreeObserver observer = listview.ViewTreeObserver;
            observer.AddOnPreDrawListener(new CustomOnPreDrawListener(listview, observer, adapter, mItemIdTopMap, moveView));
        }

        // ================================================================================ 
        // Interface declaration
        // ================================================================================ 

        /**
         * Utility, to avoid having to implement every method in AnimationListener in
         * every implementation class
         */
        public class AnimationListenerAdapter : Java.Lang.Object, IAnimationListener
        {


            public void OnAnimationEnd(Animation animation) { }

            public void OnAnimationRepeat(Animation animation) { }

            public void OnAnimationStart(Animation animation) { }

        }

        // ================================================================================ 
        // Honeycomb support
        // ================================================================================ 

        /**
         * Returns true if the current runtime is Honeycomb or later
         */
        private bool isRuntimePostGingerbread()
        {
            return Android.OS.Build.VERSION.SdkInt >= Android.OS.Build.VERSION_CODES.Honeycomb;
        }

        /**
         * Animate a view between start and end X/Y locations, using either old (pre-3.0) or
         * new animation APIs.
         */

        private void moveView(View view, float startX, float endX, float startY, float endY, Runnable endAction)
        {
            Runnable finalEndAction = endAction;
            if (isRuntimePostGingerbread())
            {
                view.Animate().SetDuration(MOVE_DURATION);
                if (startX != endX)
                {
                    ObjectAnimator anim = ObjectAnimator.OfFloat(view, View.X, startX, endX);
                    anim.SetDuration(MOVE_DURATION);
                    anim.Start();
                    setAnimatorEndAction(anim, endAction);
                    endAction = null;
                }
                if (startY != endY)
                {
                    ObjectAnimator anim = ObjectAnimator.OfFloat(view, View.Y, startY, endY);
                    anim.SetDuration(MOVE_DURATION);
                    anim.Start();
                    setAnimatorEndAction(anim, endAction);
                }
            }
            else
            {
                TranslateAnimation translator = new TranslateAnimation(startX, endX, startY, endY);
                translator.Duration = (MOVE_DURATION);
                view.StartAnimation(translator);
                if (endAction != null)
                {
                    view.Animation.SetAnimationListener(new CustomAnimatonListenerAdapter(finalEndAction));
                }
            }
        }


        private void setAnimatorEndAction(Animator animator, Runnable endAction)
        {
            if (endAction != null)
            {
                animator.AddListener(new CustomAnimatorListenerAdapter(endAction));
            }
        }

    }
}