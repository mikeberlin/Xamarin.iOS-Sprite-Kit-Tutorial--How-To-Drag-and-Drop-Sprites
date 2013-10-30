using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.SpriteKit;
using MonoTouch.UIKit;

namespace DragDrop
{
	public class MyScene : SKScene
	{
		#region Constants and Properties

		private const string ANIMAL_NODE_NAME = "movable";

		private SKSpriteNode Background { get; set; }
		private SKSpriteNode SelectedNode { get; set; }

		#endregion

		#region Constructors

		public MyScene (SizeF size)
		{
			this.Size = size;

			// Load the background
			this.Background = new SKSpriteNode ("blue-shooting-stars");
			this.Background.Name = "background";
			this.Background.AnchorPoint = new PointF (0, 0);
			this.AddChild (Background);

			// Loading the images
			List<string> imageNames = new List<string>() {"bird", "cat", "dog", "turtle"};

			int i = 0;
			foreach (string imageName in imageNames) {
				float offsetFraction = ((float)(i++ + 1) / (imageNames.Count + 1));

				SKSpriteNode sprite = new SKSpriteNode (imageName);
				sprite.Name = ANIMAL_NODE_NAME;
				sprite.Position = new PointF (size.Width * offsetFraction, size.Height / 2);

				this.Background.AddChild (sprite);
			}
		}

		#endregion

		#region SKScene Events
		/*
		public override void TouchesBegan (NSSet touches, UIEvent e)
		{
			base.TouchesBegan (touches, e);

			UITouch touch = (UITouch)touches.AnyObject;
			PointF positionInScene = touch.LocationInNode (this);
			this.SelectNodeForTouch (positionInScene);
		}

		public override void TouchesMoved(NSSet touches, UIEvent e)
		{
			base.TouchesMoved (touches, e);

			UITouch touch = (UITouch)touches.AnyObject;
			PointF positionInScene = touch.LocationInNode (this);
			PointF previousPosition = touch.PreviousLocationInNode (this);

			PointF translation = new PointF (positionInScene.X - previousPosition.X, positionInScene.Y - previousPosition.Y);
			this.PanForTranslation (translation);
		}
		*/

		public override void DidMoveToView(SKView view)
		{
			base.DidMoveToView (view);

			UIPanGestureRecognizer gestureRecognizer = new UIPanGestureRecognizer (HandlePanFrom);
			this.View.AddGestureRecognizer (gestureRecognizer);
		}

		#endregion

		#region Private Methods

		private void SelectNodeForTouch(PointF touchLocation)
		{
			// get the node that is on the posiion touchLocation
			SKSpriteNode touchedNode = (SKSpriteNode)this.GetNodeAtPoint(touchLocation);

			// if this is a new node being touched then stop all previous animation and set it as the selected node.
			if (this.SelectedNode != touchedNode) {
				if (this.SelectedNode != null) {
					this.SelectedNode.RemoveAllActions ();
					this.SelectedNode.RunAction (SKAction.RotateToAngle (0.0f, 0.1));
				}

				this.SelectedNode = touchedNode;

				// if the node we've touched is an animal node then make it wiggle y0!
				if (touchedNode.Name == ANIMAL_NODE_NAME) {
					SKAction sequence = SKAction.Sequence (new SKAction[] {
						SKAction.RotateByAngle(this.DegreeToRadian(-4.0f), 0.1),
						SKAction.RotateByAngle(0.0f, 0.1),
						SKAction.RotateByAngle(this.DegreeToRadian(4.0f), 0.1)
					});

					this.SelectedNode.RunAction(SKAction.RepeatActionForever(sequence));
				}
			}
		}

		private float DegreeToRadian(float degree)
		{
			return (degree / 180.0f * (float)Math.PI);
		}

		private PointF BoundLayerPosition(PointF finalPosition)
		{
			SizeF windowSize = this.Size;

			PointF returnValue = finalPosition;
			returnValue.X = Math.Min (returnValue.X, 0);
			returnValue.X = Math.Max (returnValue.X, -this.Background.Size.Width + windowSize.Width);
			returnValue.Y = this.Position.Y;

			return returnValue;
		}

		private void PanForTranslation(PointF translation)
		{
			PointF position = this.SelectedNode.Position;
			PointF translatedPosition = new PointF (position.X + translation.X, position.Y + translation.Y);

			if (this.SelectedNode.Name == ANIMAL_NODE_NAME) {
				this.SelectedNode.Position = translatedPosition;
			}
			else {
				this.Background.Position = this.BoundLayerPosition (translatedPosition);
			}
		}

		private void HandlePanFrom(UIPanGestureRecognizer recognizer)
		{
			if (recognizer.State == UIGestureRecognizerState.Began) {
				PointF touchLocation = recognizer.LocationInView (recognizer.View);
				touchLocation = this.ConvertPointFromView (touchLocation);
				this.SelectNodeForTouch (touchLocation);
			}
			else if (recognizer.State == UIGestureRecognizerState.Changed) {
				PointF translation = recognizer.TranslationInView (recognizer.View);
				translation = new PointF (translation.X, -translation.Y);
				this.PanForTranslation (translation);
				recognizer.SetTranslation (new PointF (0, 0), recognizer.View);
			}
			else if (recognizer.State == UIGestureRecognizerState.Ended) {
				float scrollDuration = 0.2f;

				if (this.SelectedNode.Name != ANIMAL_NODE_NAME) {
					PointF velocity = recognizer.VelocityInView (recognizer.View);
					PointF p = mult (velocity, scrollDuration);

					PointF position = this.SelectedNode.Position;
					PointF newPos = new PointF (position.X + p.X, position.Y + p.Y);
					newPos = this.BoundLayerPosition (newPos);
					this.SelectedNode.RemoveAllActions ();

					SKAction moveTo = SKAction.MoveTo (newPos, scrollDuration);
					moveTo.TimingMode = SKActionTimingMode.EaseOut;
					this.SelectedNode.RunAction (moveTo);
				}
			}
		}

		private PointF mult(PointF v, float s)
		{
			return new PointF (v.X * s, v.Y * s);
		}

		#endregion
	}
}