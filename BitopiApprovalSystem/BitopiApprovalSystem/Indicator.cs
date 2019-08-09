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
using Android.Graphics.Drawables;
using Android.Animation;
using static Android.Animation.ValueAnimator;
using Android.Graphics;
using Android.Views.Animations;
using System.Threading.Tasks;
using Java.Lang;
using Android.Util;
using Android.Content.Res;
using Android.Text;

namespace BitopiApprovalSystem.BitopiIndicator
{
    public class BallZigZagIndicator : CustomIndicator
    {
        float[] translateX = new float[2], translateY = new float[2];
        public override void draw(Canvas canvas, Paint paint)
        {
            for (int i = 0; i < 2; i++)
            {
                canvas.Save();
                canvas.Translate(translateX[i], translateY[i]);
                canvas.DrawCircle(0, 0, getWidth() / 10, paint);
                canvas.Restore();
            }
        }
        public override List<ValueAnimator> onCreateAnimators()
        {
            List<ValueAnimator> animators = new List<ValueAnimator>();
            float startX = getWidth() / 6;
            float startY = getWidth() / 6;
            for (int i = 0; i < 2; i++)
            {
                int index = i;
                ValueAnimator translateXAnim = ValueAnimator.OfFloat(startX, getWidth() - startX, getWidth() / 2, startX);
                if (i == 1)
                {
                    translateXAnim = ValueAnimator.OfFloat(getWidth() - startX, startX, getWidth() / 2, getWidth() - startX);
                }
                ValueAnimator translateYAnim = ValueAnimator.OfFloat(startY, startY, getHeight() / 2, startY);
                if (i == 1)
                {
                    translateYAnim = ValueAnimator.OfFloat(getHeight() - startY, getHeight() - startY, getHeight() / 2, getHeight() - startY);
                }
                translateXAnim.SetDuration(1000);
                translateXAnim.SetInterpolator(new LinearInterpolator());
                translateXAnim.RepeatCount = (-1);
                addUpdateListener(translateXAnim, new AnimatorUpdateListener(translateX, index, postInvalidate));
                translateYAnim.SetDuration(1000);
                translateYAnim.SetInterpolator(new LinearInterpolator());
                translateYAnim.RepeatCount = (-1);
                addUpdateListener(translateYAnim, new AnimatorUpdateListener(translateX, index, postInvalidate));
                animators.Add(translateXAnim);
                animators.Add(translateYAnim);
            }
            return animators;
        }
    }
    public class CustomIndicator : Drawable, IAnimatable
    {
        private Dictionary<ValueAnimator, IAnimatorUpdateListener> mUpdateListeners = new Dictionary<ValueAnimator, IAnimatorUpdateListener>();
        private List<ValueAnimator> mAnimators;
        private int alpha = 255;
        private static Rect ZERO_BOUNDS_RECT = new Rect();
        protected Rect drawBounds = ZERO_BOUNDS_RECT;
        private bool mHasAnimators;

        private Paint mPaint = new Paint();
        public CustomIndicator()
        {
            mPaint.Color = (Color.White);
            mPaint.SetStyle(Paint.Style.Fill);
            mPaint.AntiAlias = (true);
        }
        public int getColor()
        {
            return mPaint.Color;
        }
        public override void SetAlpha(int alpha)
        {
            this.alpha = alpha;
        }
        public override int Alpha
        {
            get
            {
                return this.alpha;
            }

            set
            {
                this.alpha = value;
            }
        }
        public override void SetColorFilter(ColorFilter colorFilter)
        {

        }
        public override int Opacity
        {
            get
            {
                return (int)Format.Opaque;
            }
        }

        public override void Draw(Canvas canvas)
        {
            draw(canvas, mPaint);
        }
        public virtual void draw(Canvas canvas, Paint paint) { }
        public virtual List<ValueAnimator> onCreateAnimators() { return null; }

        public void setColor(int color)
        {
            mPaint.Color = new Color(color);
        }
        public bool IsRunning
        {
            get
            {
                foreach (var animator in mAnimators)
                {
                    return animator.IsRunning;
                }
                return false;
            }
        }

        public void Start()
        {
            ensureAnimators();

            if (mAnimators == null)
            {
                return;
            }

            // If the animators has not ended, do nothing.
            if (isStarted())
            {
                return;
            }
            startAnimators();
            InvalidateSelf();
        }
        private void startAnimators()
        {
            for (int i = 0; i < mAnimators.Count(); i++)
            {
                ValueAnimator animator = mAnimators[(i)];

                //when the animator restart , add the updateListener again because they
                // was removed by animator stop .
                ValueAnimator.IAnimatorUpdateListener updateListener = mUpdateListeners[animator];
                if (updateListener != null)
                {
                    animator.AddUpdateListener(updateListener);
                }

                animator.Start();
            }
        }
        private void stopAnimators()
        {
            if (mAnimators != null)
            {
                foreach (var animator in mAnimators)
                {
                    if (animator != null && animator.IsStarted)
                    {
                        animator.RemoveAllUpdateListeners();
                        animator.End();
                    }
                }
            }
        }
        private void ensureAnimators()
        {
            if (!mHasAnimators)
            {
                mAnimators = onCreateAnimators();
                mHasAnimators = true;
            }
        }
        public void Stop()
        {
            stopAnimators();
        }
        private bool isStarted()
        {
            foreach (var animator in mAnimators)
            {
                return animator.IsStarted;
            }
            return false;
        }
        public void addUpdateListener(ValueAnimator animator, ValueAnimator.IAnimatorUpdateListener updateListener)
        {
            mUpdateListeners.Add(animator, updateListener);
        }
        protected override void OnBoundsChange(Rect bounds)
        {
            base.OnBoundsChange(bounds);
            setDrawBounds(bounds);
        }
        public void setDrawBounds(Rect drawBounds)
        {
            setDrawBounds(drawBounds.Left, drawBounds.Top, drawBounds.Right, drawBounds.Bottom);
        }

        public void setDrawBounds(int left, int top, int right, int bottom)
        {
            this.drawBounds = new Rect(left, top, right, bottom);
        }

        public void postInvalidate()
        {
            InvalidateSelf();
        }

        public Rect getDrawBounds()
        {
            return drawBounds;
        }

        public int getWidth()
        {
            return drawBounds.Width();
        }

        public int getHeight()
        {
            return drawBounds.Height();
        }

        public int centerX()
        {
            return drawBounds.CenterX();
        }

        public int centerY()
        {
            return drawBounds.CenterY();
        }

        public float exactCenterX()
        {
            return drawBounds.ExactCenterX();
        }

        public float exactCenterY()
        {
            return drawBounds.ExactCenterY();
        }
    }
    public class AnimatorUpdateListener : Java.Lang.Object,IAnimatorUpdateListener
    {
        float[] translateX;
        int index;
        Action postInvalidate;
        public AnimatorUpdateListener(float[] _translateX, int _index, Action _postInvalidate)
        {
            translateX = _translateX;
            index = _index;
            postInvalidate = _postInvalidate;
        }
        public IntPtr Handle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            translateX[index] = (float)animation.AnimatedValue;
            postInvalidate();
        }
    }
    [Register("BitopiApprovalSystem.BitopiIndicator.AVLoadingIndicatorView")]
    public class AVLoadingIndicatorView : View
    {
        private static string TAG = "AVLoadingIndicatorView";
        

        private static BallZigZagIndicator DEFAULT_INDICATOR = new BallZigZagIndicator();

        private static int MIN_SHOW_TIME = 500; // ms
        private static int MIN_DELAY = 500; // ms

        private static long mStartTime = -1;

        private static bool mPostedHide = false;

        private static bool mPostedShow = false;

        private static bool mDismissed = false;
        
        private Runnable mDelayedHide = new Runnable(() =>
        {
            mPostedHide = false;
            mStartTime = -1;
            //Visibility = (ViewStates.Gone);
        });
        private Runnable mDelayedShow = new Runnable(() =>
        {
            mPostedShow = false;
            if (!mDismissed)
            {
                mStartTime = DateTime.Now.Millisecond;
                //Visibility=(ViewStates.Visible);
            }
        });
        int mMinWidth;
        int mMaxWidth;
        int mMinHeight;
        int mMaxHeight;

        private CustomIndicator mIndicator;
        private int mIndicatorColor;

        private bool mShouldStartAnimationDrawable;
        public AVLoadingIndicatorView(Context context) : base(context)
        {

            init(context, null, 0, 0);
        }

        public AVLoadingIndicatorView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

            init(context, attrs, 0, Resource.Style.AVLoadingIndicatorView);

        }

        public AVLoadingIndicatorView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {

            init(context, attrs, defStyleAttr, Resource.Style.AVLoadingIndicatorView);
        }
        public AVLoadingIndicatorView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {

            init(context, attrs, defStyleAttr, Resource.Style.AVLoadingIndicatorView);
        }
        private void init(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
        {
            mMinWidth = 24;
            mMaxWidth = 48;
            mMinHeight = 24;
            mMaxHeight = 48;

            TypedArray a = context.ObtainStyledAttributes(
                   attrs, Resource.Styleable.AVLoadingIndicatorView, defStyleAttr, defStyleRes);
            

            mMinWidth = a.GetDimensionPixelSize(Resource.Styleable.AVLoadingIndicatorView_minWidth, mMinWidth);
            mMaxWidth = a.GetDimensionPixelSize(Resource.Styleable.AVLoadingIndicatorView_maxWidth, mMaxWidth);
            mMinHeight = a.GetDimensionPixelSize(Resource.Styleable.AVLoadingIndicatorView_minHeight, mMinHeight);
            mMaxHeight = a.GetDimensionPixelSize(Resource.Styleable.AVLoadingIndicatorView_maxHeight, mMaxHeight);
            string indicatorName = a.GetString(Resource.Styleable.AVLoadingIndicatorView_indicatorName);
            mIndicatorColor = a.GetColor(Resource.Styleable.AVLoadingIndicatorView_indicatorColor, Color.White);
            setIndicator(indicatorName);
            if (mIndicator == null)
            {
                setIndicator(DEFAULT_INDICATOR);
            }
            a.Recycle();
        }
        public CustomIndicator getIndicator()
        {
            return mIndicator;
        }
        public void setIndicator(CustomIndicator d)
        {
            if (mIndicator != d)
            {
                if (mIndicator != null)
                {
                    mIndicator.SetCallback(null);
                    UnscheduleDrawable(mIndicator);
                }

                mIndicator = d;
                //need to set indicator color again if you didn't specified when you update the indicator .
                setIndicatorColor(mIndicatorColor);
                if (d != null)
                {
                    d.SetCallback(this);
                }
                PostInvalidate();
            }
        }
        public void setIndicatorColor(int color)
        {
            this.mIndicatorColor = color;
            mIndicator.setColor(color);
        }
        public void setIndicator(string indicatorName)
        {
            if (TextUtils.IsEmpty(indicatorName))
            {
                return;
            }
            Java.Lang.StringBuilder drawableClassName = new Java.Lang.StringBuilder();
            if (!indicatorName.Contains("."))
            {
                string defaultPackageName = Class.Package.Name;
                drawableClassName.Append(defaultPackageName)
                        .Append(".indicators")
                        .Append(".");
            }
            drawableClassName.Append(indicatorName);
            try
            {
                Class drawableClass = Class.ForName(drawableClassName.ToString());
                CustomIndicator indicator = (CustomIndicator)drawableClass.NewInstance();
                setIndicator(indicator);
            }
            catch (ClassNotFoundException e)
            {
                //Log.e(TAG, "Didn't find your class , check the name again !");
            }
            catch (InstantiationException e)
            {

            }
            catch (IllegalAccessException e)
            {
                //e.printStackTrace();
            }
        }
        public void smoothToShow()
        {
            StartAnimation(AnimationUtils.LoadAnimation(Context, Android.Resource.Animation.FadeIn));
            Visibility = (ViewStates.Visible);
        }

        public void smoothToHide()
        {
            StartAnimation(AnimationUtils.LoadAnimation(Context, Android.Resource.Animation.FadeOut));
            Visibility=(ViewStates.Gone);
        }

        public void hide()
        {
            mDismissed = true;
            RemoveCallbacks(mDelayedShow);
            long diff = DateTime.Now.Millisecond - mStartTime;
            if (diff >= MIN_SHOW_TIME || mStartTime == -1)
            {
                // The progress spinner has been shown long enough
                // OR was not shown yet. If it wasn't shown yet,
                // it will just never be shown.
                Visibility=(ViewStates.Gone);
            }
            else
            {
                // The progress spinner is shown, but not long enough,
                // so put a delayed message in to hide it when its been
                // shown long enough.
                if (!mPostedHide)
                {
                    PostDelayed(mDelayedHide, MIN_SHOW_TIME - diff);
                    mPostedHide = true;
                }
            }
        }

        public void show()
        {
            // Reset the start time.
            mStartTime = -1;
            mDismissed = false;
            RemoveCallbacks(mDelayedHide);
            if (!mPostedShow)
            {
                PostDelayed(mDelayedShow, MIN_DELAY);
                mPostedShow = true;
            }
        }
        protected override bool VerifyDrawable(Drawable who)
        {
            return who == mIndicator
|| base.VerifyDrawable(who);
        }
        void startAnimation()
        {
            if (Visibility != ViewStates.Visible)
            {
                return;
            }

            if (mIndicator is IAnimatable) {
                mShouldStartAnimationDrawable = true;
            }
            PostInvalidate();
        }
        void stopAnimation()
        {
            if (mIndicator is IAnimatable) {
                mIndicator.Stop();
                mShouldStartAnimationDrawable = false;
            }
            PostInvalidate();
        }
        public override ViewStates Visibility
        {
            get
            {
                return base.Visibility;
            }

            set
            {
                if (Visibility != value)
                {
                    base.Visibility = value;
                    if (value == ViewStates.Gone || value == ViewStates.Invisible)
                    {
                        stopAnimation();
                    }
                    else
                    {
                        startAnimation();
                    }
                }
            }
        }
        protected override void OnVisibilityChanged(View changedView, [GeneratedEnum] ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);
            
            if (visibility == ViewStates.Gone || visibility == ViewStates.Invisible)
            {
                stopAnimation();
            }
            else
            {
                startAnimation();
            }
        }
        public override void InvalidateDrawable(Drawable drawable)
        {
            if (VerifyDrawable(drawable))
            {
                 Rect dirty = drawable.Bounds;
                 int scrollX = ScrollX + PaddingLeft; 
                 int scrollY = ScrollY+ PaddingTop;

                Invalidate(dirty.Left + scrollX, dirty.Top + scrollY,
                        dirty.Right + scrollX, dirty.Bottom + scrollY);
            }
            else
            {
                base.InvalidateDrawable(drawable);
            }
            
        }
        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            updateDrawableBounds(w, h);
        }
        private void updateDrawableBounds(int w, int h)
        {
            // onDraw will translate the canvas so we draw starting at 0,0.
            // Subtract out padding for the purposes of the calculations below.
            w -= PaddingRight + PaddingLeft;
            h -= PaddingTop + PaddingBottom;

            int right = w;
            int bottom = h;
            int top = 0;
            int left = 0;

            if (mIndicator != null)
            {
                // Maintain aspect ratio. Certain kinds of animated drawables
                // get very confused otherwise.
                 int intrinsicWidth = mIndicator.IntrinsicWidth;
                 int intrinsicHeight = mIndicator.IntrinsicHeight;
                 float intrinsicAspect = (float)intrinsicWidth / intrinsicHeight;
                 float boundAspect = (float)w / h;
                if (intrinsicAspect != boundAspect)
                {
                    if (boundAspect > intrinsicAspect)
                    {
                        // New width is larger. Make it smaller to match height.
                          int width = (int)(h * intrinsicAspect);
                        left = (w - width) / 2;
                        right = left + width;
                    }
                    else
                    {
                        // New height is larger. Make it smaller to match width.
                          int height = (int)(w * (1 / intrinsicAspect));
                        top = (h - height) / 2;
                        bottom = top + height;
                    }
                }
                mIndicator.SetBounds(left, top, right, bottom);
            }
        }
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            
            drawTrack(canvas);
        }
        void drawTrack(Canvas canvas)
        {
             Drawable d = mIndicator;
            if (d != null)
            {
                // Translate canvas so a indeterminate circular progress bar with padding
                // rotates properly in its animation
                 int saveCount = canvas.Save();

                canvas.Translate(PaddingLeft, PaddingTop);

                d.Draw(canvas);
                canvas.RestoreToCount(saveCount);

                if (mShouldStartAnimationDrawable && d is IAnimatable) {
                    ((IAnimatable)d).Start();
                    mShouldStartAnimationDrawable = false;
                }
            }
        }
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            int dw = 0;
            int dh = 0;

             Drawable d = mIndicator;
            if (d != null)
            {
                dw = Java.Lang.Math.Max(mMinWidth, Java.Lang.Math.Min(mMaxWidth, d.IntrinsicWidth));
                dh = Java.Lang.Math.Max(mMinHeight, Java.Lang.Math.Min(mMaxHeight, d.IntrinsicHeight));
            }

            updateDrawableState();

            dw += PaddingLeft + PaddingRight;
            dh += PaddingTop + PaddingBottom;

             int measuredWidth = ResolveSizeAndState(dw, widthMeasureSpec, 0);
             int measuredHeight = ResolveSizeAndState(dh, heightMeasureSpec, 0);
            SetMeasuredDimension(measuredWidth, measuredHeight);
        }
        protected override void DrawableStateChanged()
        {
            base.DrawableStateChanged();
            updateDrawableState();
        }
        private void updateDrawableState()
        {
             int[] state = GetDrawableState();
            if (mIndicator != null && mIndicator.IsStateful)
            {
                mIndicator.SetState(state);
            }
        }
        public override void DrawableHotspotChanged(float x, float y)
        {
            base.DrawableHotspotChanged(x, y);
            if (mIndicator != null)
            {
                mIndicator.SetHotspot(x, y);
            }
        }
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            startAnimation();
            removeCallbacks();
        }
        protected override void OnDetachedFromWindow()
        {
            stopAnimation();
            base.OnDetachedFromWindow();
            removeCallbacks();
        }
        private void removeCallbacks()
        {
            RemoveCallbacks(mDelayedHide);
            RemoveCallbacks(mDelayedShow);
            
        }
    }
}