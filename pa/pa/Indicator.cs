//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Android.Graphics.Drawables;
//using Android.Animation;
//using Android.Graphics;

//namespace pa
//{
//    public abstract class Indicator : Drawable , IAnimatable
//    {

//        private Dictionary<ValueAnimator, ValueAnimator.IAnimatorUpdateListener> mUpdateListeners = new Dictionary<ValueAnimator, ValueAnimator.IAnimatorUpdateListener>();

//    private List<ValueAnimator> mAnimators;
//    private int alpha = 255;
//    private static  Rect ZERO_BOUNDS_RECT = new Rect();
//    protected Rect drawBounds = ZERO_BOUNDS_RECT;

//    private bool mHasAnimators;

//    private Paint mPaint = new Paint();

//    public Indicator()
//    {
//        mPaint.Color=(Color.White);
//        mPaint.SetStyle(Paint.Style.Fill);
//        mPaint.AntiAlias=(true);
//    }

//    public int getColor()
//    {
//        return mPaint.Color;
//    }

//    public void setColor(int color)
//    {
//        mPaint.Color=(Color)(color);
//    }

//        public override void SetAlpha(int alpha)
//        {
//            this.alpha = alpha;
//        }

//        public override int Alpha
//        {
//            get
//            {
//                return base.Alpha;
//            }

//            set
//            {
//                base.Alpha = value;
//            }
//        }
//        public override int Opacity
//        {
//            get
//            {
//                return 1;
//            }
//        }


//        public override ColorFilter ColorFilter
//        {
//            get
//            {
//                return base.ColorFilter;
//            }
//        }

//        public bool IsRunning
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public override void Draw(Canvas canvas)
//        {
//            draw(canvas, mPaint);
//        }

//    public abstract void draw(Canvas canvas, Paint paint);

//    public abstract List<ValueAnimator> onCreateAnimators();

//    public  void Start()
//    {
//        ensureAnimators();

//        if (mAnimators == null)
//        {
//            return;
//        }

//        // If the animators has not ended, do nothing.
//        if (isStarted())
//        {
//            return;
//        }
//        startAnimators();
//        invalidateSelf();
//    }

//    private void startAnimators()
//    {
//        for (int i = 0; i < mAnimators.size; i++)
//        {
//            ValueAnimator animator = mAnimators.get(i);

//            //when the animator restart , add the updateListener again because they
//            // was removed by animator stop .
//            ValueAnimator.AnimatorUpdateListener updateListener = mUpdateListeners.get(animator);
//            if (updateListener != null)
//            {
//                animator.addUpdateListener(updateListener);
//            }

//            animator.start();
//        }
//    }

//    private void stopAnimators()
//    {
//        if (mAnimators != null)
//        {
//            for (ValueAnimator animator : mAnimators)
//            {
//                if (animator != null && animator.isStarted())
//                {
//                    animator.removeAllUpdateListeners();
//                    animator.end();
//                }
//            }
//        }
//    }

//    private void ensureAnimators()
//    {
//        if (!mHasAnimators)
//        {
//            mAnimators = onCreateAnimators();
//            mHasAnimators = true;
//        }
//    }

//    @Override
//    public void stop()
//    {
//        stopAnimators();
//    }

//    private bool isStarted()
//    {
//        for (ValueAnimator animator : mAnimators)
//        {
//            return animator.isStarted();
//        }
//        return false;
//    }

//    @Override
//    public bool isRunning()
//    {
//        for (ValueAnimator animator : mAnimators)
//        {
//            return animator.isRunning();
//        }
//        return false;
//    }

//    /**
//     *  Your should use this to add AnimatorUpdateListener when
//     *  create animator , otherwise , animator doesn't work when
//     *  the animation restart .
//     * @param updateListener
//     */
//    public void addUpdateListener(ValueAnimator animator, ValueAnimator.AnimatorUpdateListener updateListener)
//    {
//        mUpdateListeners.put(animator, updateListener);
//    }

//    @Override
//    protected void onBoundsChange(Rect bounds)
//    {
//        super.onBoundsChange(bounds);
//        setDrawBounds(bounds);
//    }

//    public void setDrawBounds(Rect drawBounds)
//    {
//        setDrawBounds(drawBounds.left, drawBounds.top, drawBounds.right, drawBounds.bottom);
//    }

//    public void setDrawBounds(int left, int top, int right, int bottom)
//    {
//        this.drawBounds = new Rect(left, top, right, bottom);
//    }

//    public void postInvalidate()
//    {
//        invalidateSelf();
//    }

//    public Rect getDrawBounds()
//    {
//        return drawBounds;
//    }

//    public int getWidth()
//    {
//        return drawBounds.width();
//    }

//    public int getHeight()
//    {
//        return drawBounds.height();
//    }

//    public int centerX()
//    {
//        return drawBounds.centerX();
//    }

//    public int centerY()
//    {
//        return drawBounds.centerY();
//    }

//    public float exactCenterX()
//    {
//        return drawBounds.exactCenterX();
//    }

//    public float exactCenterY()
//    {
//        return drawBounds.exactCenterY();
//    }

        
    