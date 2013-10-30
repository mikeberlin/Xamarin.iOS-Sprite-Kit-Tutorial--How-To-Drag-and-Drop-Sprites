using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;

namespace DragDrop
{
	public partial class ViewController : UIViewController
	{
		public ViewController()
		{
		}

		public override void LoadView()
		{
			base.LoadView ();

			this.View = new SKView {
				ShowsFPS = true,
				ShowsDrawCount = true
			};
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews ();

			// Configure the view
			SKView skView = (SKView)this.View;

			if (skView.Scene == null) {
				// Create and configure the scene
				SKScene scene = new MyScene (skView.Bounds.Size);
				scene.ScaleMode = SKSceneScaleMode.AspectFill;

				// Present the scene
				skView.PresentScene (scene);
			}
		}

		public override bool PrefersStatusBarHidden()
		{
			return true;
		}
	}
}