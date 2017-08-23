
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Java.IO;

namespace pa.CustomControl
{
    public class CustomGifView : View
    {

        private System.IO.Stream gifInputStream;
        private Movie gifMovie;
        private int movieWidth, movieHeight;
        private long movieDuration;
        private long mMovieStart;

        public CustomGifView(Context context) : base(context)
        {

            init(context);
        }

        public CustomGifView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

            init(context);
        }

        public CustomGifView(Context context, IAttributeSet attrs,
          int defStyleAttr) : base(context, attrs, defStyleAttr)
        {

            init(context);
        }

        private void init(Context context)
        {
            Focusable = true;

            gifInputStream = context.Resources
              .OpenRawResource(pa.Resource.Drawable.loader);

            gifMovie = Movie.DecodeStream(gifInputStream);
            movieWidth = gifMovie.Width();
            movieHeight = gifMovie.Height();
            movieDuration = gifMovie.Duration();
        }


        protected override void OnMeasure(int widthMeasureSpec,
   int heightMeasureSpec)
        {
            SetMeasuredDimension(movieWidth, movieHeight);
        }

        public int getMovieWidth()
        {
            return movieWidth;
        }

        public int getMovieHeight()
        {
            return movieHeight;
        }

        public long getMovieDuration()
        {
            return movieDuration;
        }


        protected override void OnDraw(Canvas canvas)
        {

            long now = Android.OS.SystemClock.UptimeMillis();
            if (mMovieStart == 0)
            {   // first time
                mMovieStart = now;
            }

            if (gifMovie != null)
            {

                int dur = gifMovie.Duration();
                if (dur == 0)
                {
                    dur = 1000;
                }

                int relTime = (int)((now - mMovieStart) % dur);

                gifMovie.SetTime(relTime);

                gifMovie.Draw(canvas, 0, 0);
                Invalidate();

            }

        }

    }
}