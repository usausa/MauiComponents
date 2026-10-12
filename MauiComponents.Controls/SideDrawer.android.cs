namespace MauiComponents.Controls;

using Android.Content;
using Android.Views;

using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

public sealed partial class SideDrawer
{
    private const double ExclusionHeight = 200;

    //--------------------------------------------------------------------------------
    // Platform
    //--------------------------------------------------------------------------------

    static SideDrawer()
    {
        var factory = ViewHandler<Microsoft.Maui.ILayout, LayoutViewGroup>.PlatformViewFactory;
        ViewHandler<Microsoft.Maui.ILayout, LayoutViewGroup>.PlatformViewFactory = handler =>
            handler.VirtualView is DrawerPanel panel ? new PanelView(handler.MauiContext!.Context!, panel.Owner) : factory?.Invoke(handler)!;
    }

    private partial Grid CreatePanel() => new DrawerPanel(this);

    partial void UpdateGestureExclusion()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            return;
        }

        if ((edge.Handler?.PlatformView is not Android.Views.View view) || (view.Context is null) || (edge.Height <= 0))
        {
            return;
        }

        var width = (int)view.Context.ToPixels(edge.Width);
        var height = (int)view.Context.ToPixels(edge.Height);
        var band = (int)view.Context.ToPixels(Math.Min(ExclusionHeight, edge.Height));
        var top = (height - band) / 2;
        using var rect = new Android.Graphics.Rect(0, top, width, top + band);
        view.SystemGestureExclusionRects = [rect];
    }

    //--------------------------------------------------------------------------------
    // Panel
    //--------------------------------------------------------------------------------

    private sealed class DrawerPanel : Grid
    {
        public SideDrawer Owner { get; }

        public DrawerPanel(SideDrawer owner)
        {
            Owner = owner;
        }
    }

    private sealed class PanelView : LayoutViewGroup
    {
        private readonly SideDrawer drawer;

        private readonly int touchSlop;

        private float startX;

        private float startY;

        private bool dragging;

        public PanelView(Context context, SideDrawer drawer)
            : base(context)
        {
            this.drawer = drawer;
            touchSlop = ViewConfiguration.Get(context)!.ScaledTouchSlop;
        }

        public override bool OnInterceptTouchEvent(MotionEvent? ev)
        {
            if (ev is not null)
            {
                if (ev.ActionMasked == MotionEventActions.Down)
                {
                    Down(ev);
                }
                else if ((ev.ActionMasked == MotionEventActions.Move) && !dragging && IsDrag(ev) && (ev.RawX < startX))
                {
                    BeginDrag(ev);
                }
            }

            return dragging;
        }

        public override bool OnTouchEvent(MotionEvent? e)
        {
            if (e is not null)
            {
                switch (e.ActionMasked)
                {
                    case MotionEventActions.Down:
                        Down(e);
                        break;
                    case MotionEventActions.Move:
                        if (!dragging && IsDrag(e))
                        {
                            BeginDrag(e);
                        }

                        if (dragging)
                        {
                            drawer.MoveDrag(Context.FromPixels(e.RawX - startX));
                        }

                        break;
                    case MotionEventActions.Up:
                    case MotionEventActions.Cancel:
                        if (dragging)
                        {
                            dragging = false;
                            drawer.Settle();
                        }

                        break;
                }
            }

            return true;
        }

        private void Down(MotionEvent e)
        {
            startX = e.RawX;
            startY = e.RawY;
            dragging = false;
        }

        private bool IsDrag(MotionEvent e)
        {
            var dx = Math.Abs(e.RawX - startX);
            return (dx > touchSlop) && (dx > Math.Abs(e.RawY - startY));
        }

        private void BeginDrag(MotionEvent e)
        {
            dragging = true;
            startX = e.RawX;
            Parent?.RequestDisallowInterceptTouchEvent(true);
            drawer.StartDrag();
        }
    }
}
