using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace HandSchool.Droid
{
    [Register("com.x90yang.handSchool.handSchool.droid.ResizeableTextView")]
    class ResizeableTextView : TextView
    {
        public ResizeableTextView(Context context) : base(context)
        {
        }

        public ResizeableTextView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public ResizeableTextView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public ResizeableTextView(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected ResizeableTextView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
    }
}