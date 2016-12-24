using Android.App;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using Android.Views;
using Android.Graphics;
using System;
using Android.Content.PM;

namespace AnimeWatcher
{
    [Activity(Label = "AnimeWatcher", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden, HardwareAccelerated = true, MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.NoTitleBar")]
    public class MainActivity : Activity
    {

        private bool trigger = false;

        private EditText url;
        private Button btn;
        private ProgressBar pb;
        private WebView wv;

        private ExtendedChromeView mWebChromeClient = null;

        public static MainActivity inst;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            inst = this;

            url = FindViewById<EditText>(Resource.Id.url);
            btn = FindViewById<Button>(Resource.Id.search);
            pb = FindViewById<ProgressBar>(Resource.Id.progressBar);
            wv = FindViewById<WebView>(Resource.Id.webView1);

            wv.SetWebViewClient(new WebViewClient());
            mWebChromeClient = new ExtendedChromeView();
            wv.SetWebChromeClient(mWebChromeClient);

            wv.LoadUrl("http://www.google.com");
            url.Text = "http://www.google.com";

            btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, System.EventArgs e)
        {
            string turi = ((url.Text.Contains("http://") || url.Text.Contains("https://"))) ? url.Text : string.Format("http://{0}", url.Text);
            Uri uri;
            bool isUri = Uri.TryCreate(turi, UriKind.Absolute, out uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
            if(isUri)
            {
                wv.LoadUrl(uri.AbsoluteUri);
            }
            else
            {
                url.Text = string.Format("http://www.google.com/search?q={0}", url.Text);
                wv.LoadUrl(string.Format("http://www.google.com/search?q={0}", url.Text));
            }
        }

        public override void OnBackPressed()
        {
            if(trigger)
            {
                MainActivity.inst.btn.Visibility = ViewStates.Visible;
                MainActivity.inst.url.Visibility = ViewStates.Visible;
                MainActivity.inst.trigger = false;
                MainActivity.inst.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            }
            else if(wv.CanGoBack())
            {
                wv.GoBack();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        private class ExtendedChromeView : WebChromeClient
        {
            FrameLayout.LayoutParams LayoutParameters = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MatchParent,
            FrameLayout.LayoutParams.MatchParent);

            public override void OnShowCustomView(View view, ICustomViewCallback callback)
            {
                MainActivity.inst.btn.Visibility = ViewStates.Gone;
                MainActivity.inst.url.Visibility = ViewStates.Gone;
                MainActivity.inst.trigger = true;
                MainActivity.inst.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
                callback.OnCustomViewHidden();
                return;
            }

            public override void OnHideCustomView() { }

            public override void OnProgressChanged(WebView view, int newProgress)
            {
                base.OnProgressChanged(view, newProgress);
                MainActivity.inst.pb.SetProgress(newProgress, true);
            }
        }

        private class ExtendedWebView : WebViewClient
        {
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                return false;
            }

            public override void OnPageStarted(WebView view, string url, Bitmap favicon)
            {
                base.OnPageStarted(view, url, favicon);
                MainActivity.inst.pb.SetProgress(0, false);
                MainActivity.inst.pb.Visibility = ViewStates.Visible;
            }

            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);
                MainActivity.inst.pb.SetProgress(0, false);
                MainActivity.inst.pb.Visibility = ViewStates.Gone;
            }
        }
    }
}

