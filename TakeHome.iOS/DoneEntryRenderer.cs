using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using TakeHome.Services;

using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using TakeHome.iOS;
using CoreGraphics;

//
//

using System.Drawing;
using System.Reflection;
using System.Runtime.Remoting.Contexts;


//
//

[assembly: ExportRenderer(typeof(DoneEntry), typeof(DoneEntryRenderer))]
namespace TakeHome.iOS
{
    public class DoneEntryRenderer : EntryRenderer
    {

        //protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        //{
        //    base.OnElementChanged(e);

        //    var toolbar = new UIToolbar(new CGRect(0.0f, 0.0f, Control.Frame.Size.Width, 44.0f));

        //    if (toolbar.Items != null)
        //    {
        //        toolbar.Items = new[]
        //        {
        //        new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
        //        new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate { Control.ResignFirstResponder(); })
        //    };
        //    }
        //    this.Control.InputAccessoryView = toolbar;
        //}

        private MethodInfo _baseEntrySendCompleted = null;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            this.AddDoneButton();
        }

        /// <summary>
        /// Add toolbar with Done button
        /// </summary>
        private void AddDoneButton()
        {
            if (Control != null)
            {
                var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

                if (Control != null)
                {
                    var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
                    {
                        this.Control.ResignFirstResponder();
                        var baseEntry = this.Element.GetType();
                        if (_baseEntrySendCompleted == null)
                        {
                            // use reflection to find our method
                            _baseEntrySendCompleted = baseEntry.GetMethod("SendCompleted", BindingFlags.NonPublic | BindingFlags.Instance);
                        }

                        try
                        {
                            _baseEntrySendCompleted.Invoke(this.Element, null);
                        }
                        catch (Exception ex)
                        {
                            // handle the invoke error condition   
                            string yuki = "Kulit";
                        }

                    });

                    toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };

                    this.Control.InputAccessoryView = toolbar;
                }

            }
        }
    }
}