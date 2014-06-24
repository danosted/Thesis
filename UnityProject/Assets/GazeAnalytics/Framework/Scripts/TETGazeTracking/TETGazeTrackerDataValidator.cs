using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TETCSharpClient.Data;
using UnityEngine;

namespace Assets.Scripts
{
	/// <summary>
	/// Utility class that maintains a run-time cache of GazeData frames. Based on the cache 
	/// the class analyzes the frame history and finds the currently valid gaze data.
	/// Use this class to avoid the 'glitch' effect of occational poor tracking.
	/// </summary>
	class TETGazeTrackerDataValidator
	{
		private double _MinimumEyesDistance = 0.1f;
		private double _MaximumEyesDistance = 0.3f;

		private FixedSizeQueue<GazeData> _Frames;

		private Eye _LastValidLeftEye;
		private Eye _LastValidRightEye;

		private Eye _LastLeftEye;
		private Eye _LastRightEye;

		private Eye _currentLeftEye;
		private Eye _currentRightEye;

		private Point2D _LastValidRawGazeCoords;
		private Point2D _LastValidSmoothedGazeCoords;
		private Point2D _LastValidUserPosition;

		private double _LastValidEyeDistance;
		private double _LastValidEyeAngle;

		private bool _isFixating;

		private float timeSinceLastValidEyeLeft;
		private float timeSinceLastValidEyeRight;

		private float timeAtLastValidEyeLeft;
		private float timeAtLastValidEyeRight;

		private float currentFixationTime;
		private float lastFixationTime;

		private static Stopwatch blinkTimer;
		private static Stopwatch fixationTimer;
		private int blinkCount;
		private int fixationIndex = 0;
		private float eyesCloseTime;
		private bool hasClosed;

		public TETGazeTrackerDataValidator(int queueLength)
		{
			_Frames = new FixedSizeQueue<GazeData>(queueLength);
			_LastValidUserPosition = new Point2D();
			blinkTimer = new Stopwatch();
			fixationTimer = new Stopwatch();
//			blinkTimer.Start();
		}

		public void Update(GazeData frame)
		{
			_Frames.Enqueue(frame);
			// update valid gazedata based on store
			Eye right = null, left = null;
			Point2D gazeCoords = null;
			Point2D gazeCoordsSmooth = null;
			GazeData gd;
			for(int i = _Frames.Count; --i >= 0;)
			{
				gd = _Frames.ElementAt(i);
				// if no tracking problems, then cache eye data
				if((gd.State & GazeData.STATE_TRACKING_FAIL) == 0 && (gd.State & GazeData.STATE_TRACKING_LOST) == 0)
				{
					if(gd.LeftEye == null)
					{
						_currentLeftEye = null;
					}
					if(gd.RightEye == null)
					{
						_currentRightEye = null;
					}
					if(null == left && null != gd.LeftEye && gd.LeftEye.PupilCenterCoordinates.X != 0 && gd.LeftEye.PupilCenterCoordinates.Y != 0)
					{
						left = gd.LeftEye;
					}
					if(null == right && null != gd.RightEye && gd.RightEye.PupilCenterCoordinates.X != 0 && gd.RightEye.PupilCenterCoordinates.Y != 0)
					{
						right = gd.RightEye;
					}

					if((left == null && _LastRightEye == null) || (left == null && right == null) || (_LastLeftEye == null && right == null) && !gd.IsFixated)
					{
						if(!blinkTimer.IsRunning)
						{
							blinkTimer.Reset();
							blinkTimer.Start();
						}
					}
					else
					{
						if(blinkTimer.IsRunning)
						{
							eyesCloseTime = (float)blinkTimer.ElapsedMilliseconds * 0.001f;
							blinkTimer.Stop();
							if(eyesCloseTime > 0.02f && eyesCloseTime < 1f)
							{
								blinkCount++;
							}
						}
					}
					_LastLeftEye = left;
					_LastRightEye = right;

					_isFixating = gd.IsFixated;
					if(!gd.IsFixated && fixationTimer.IsRunning)
					{
						lastFixationTime = (float)fixationTimer.ElapsedMilliseconds * 0.001f;
						fixationTimer.Reset();
					}
					if(!fixationTimer.IsRunning && gd.IsFixated)
					{
						fixationTimer.Start();
                        fixationIndex++;
					}

					currentFixationTime = (float)fixationTimer.ElapsedMilliseconds * 0.001f;

					if(/*(gd.State & GazeData.STATE_TRACKING_GAZE) != 0 && */null == gazeCoords && gd.RawCoordinates.X != 0 && gd.RawCoordinates.Y != 0)
					{
						gazeCoords = gd.RawCoordinates;
						gazeCoordsSmooth = gd.SmoothedCoordinates;
					}
				}
				// if gaze coordinates available, cache both raw and smoothed

				
				// break loop if valid values found
				if(null != right && null != left && null != gazeCoords)
				{
					break;
				}   
	 
			}

			if(null != left)
			{
				_LastValidLeftEye = left;
//				timeAtLastValidEyeLeft = Time.time;
			}

			if(null != right)
			{
				_LastValidRightEye = right;
//				timeAtLastValidEyeRight = Time.time;
			}

			if(null != gazeCoords)
			{
				_LastValidRawGazeCoords = gazeCoords;
				_LastValidSmoothedGazeCoords = gazeCoordsSmooth;
			}

			//Update user position values if needed data is valid
			if(null != _LastValidLeftEye && null != _LastValidRightEye)
			{
				//update user position
				lock(_LastValidUserPosition)
				{
					_LastValidUserPosition.X = (_LastValidLeftEye.PupilCenterCoordinates.X + _LastValidRightEye.PupilCenterCoordinates.X) / 2;
					_LastValidUserPosition.Y = (_LastValidLeftEye.PupilCenterCoordinates.Y + _LastValidRightEye.PupilCenterCoordinates.Y) / 2;
				}

				//update 'depth' measure
				double dist = Point2DDistance(_LastValidLeftEye, _LastValidRightEye);

				if(dist < _MinimumEyesDistance)
				{
					_MinimumEyesDistance = dist;
				}

				if(dist > _MaximumEyesDistance)
				{
					_MaximumEyesDistance = dist;
				}

				_LastValidEyeDistance = dist / (_MaximumEyesDistance - _MinimumEyesDistance);

				//update angle
				_LastValidEyeAngle = ((180 / Math.PI * Math.Atan2(_LastValidRightEye.PupilCenterCoordinates.Y - _LastValidLeftEye.PupilCenterCoordinates.Y,
                    _LastValidRightEye.PupilCenterCoordinates.X - _LastValidLeftEye.PupilCenterCoordinates.X)));

			}
		}

		private double Point2DDistance(Eye ge1, Eye ge2)
		{
			return Math.Abs(Math.Sqrt(Math.Pow(ge2.PupilCenterCoordinates.X - ge1.PupilCenterCoordinates.X, 2) + Math.Pow(ge2.PupilCenterCoordinates.Y - ge1.PupilCenterCoordinates.Y, 2)));
		}

		public Point2D GetLastValidUserPosition()
		{
			return _LastValidUserPosition;
		}

		public Eye GetLastValidLeftEye()
		{
			return _LastValidLeftEye;
		}

		public Eye GetLastValidRightEye()
		{
			return _LastValidRightEye;
		}

		public Eye GetLeftEye()
		{
			return _currentLeftEye;
		}
		
		public Eye GetRightEye()
		{
			return _currentRightEye;
		}

		public double GetLastValidUserDistance()
		{
			return _LastValidEyeDistance;
		}

		public double GetLastValidEyesAngle()
		{
			return _LastValidEyeAngle;
		}

		public Point2D GetLastValidRawGazeCoordinates()
		{
			return _LastValidRawGazeCoords;
		}

		public Point2D GetLastValidSmoothedGazeCoordinates()
		{
			return _LastValidSmoothedGazeCoords;
		}

		public float EyesCloseTime
		{
			get
			{
				return eyesCloseTime;
			}
		}

		public int BlinkCount
		{
			get
			{
				return blinkCount;
			}
		}

		public bool timeIsRunning()
		{
			return blinkTimer.IsRunning;
		}

		public bool HasClosed
		{
			get
			{
				return hasClosed;
			}
		}

		public bool isFixating()
		{
			return _isFixating;
		}

//		public float GetTimeSinceLastValidEyeLeft()
//		{
//			timeSinceLastValidEyeLeft = Time.time - timeAtLastValidEyeLeft;
//			return timeSinceLastValidEyeLeft;
//		}

//		public float GetTimeSinceLastValidEyeRight()
//		{
//			timeSinceLastValidEyeRight = Time.time - timeAtLastValidEyeRight;
//			return timeSinceLastValidEyeRight;
//		}

		public float CurrentFixationTime
		{
			get
			{
				return currentFixationTime;
			}
		}

		public float LastFixationTime 
		{
			get 
			{
				return lastFixationTime;
			}
		}

		public int FixationIndex {
			get {
				return fixationIndex;
			}
		}
	}

	class FixedSizeQueue<T> : Queue<T>
	{
		private int limit = -1;

		public int Limit
		{
			get
			{
				return limit;
			}
			set
			{
				limit = value;
			}
		}

		public FixedSizeQueue(int limit)
            : base(limit)
		{
			this.Limit = limit;
		}

		public new void Enqueue(T item)
		{
			while(this.Count >= this.Limit)
			{
				this.Dequeue();
			}
			base.Enqueue(item);
		}
	}
}
