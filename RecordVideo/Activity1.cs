// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Activity1.cs" company="">
//   
// </copyright>
// <summary>
//   The app's main Activity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RecordVideo
{
    using System;

    using Android.App;
    using Android.Hardware;
    using Android.Media;
    using Android.OS;
    using Android.Util;
    using Android.Widget;

    using Environment = Android.OS.Environment;

    /// <summary>
    /// The app's main Activity.
    /// </summary>
    [Activity(Label = "RecordVideo", MainLauncher = true)]
    public class Activity1 : Activity
    {
        /// <summary>
        /// The camera object.
        /// </summary>
        private Camera camera;

        /// <summary>
        /// The media recorder records video.
        /// </summary>
        private MediaRecorder recorder;

        /// <summary>
        /// Used to display previews, recordings, and playback.
        /// </summary>
        private VideoView video;

        #region Android Activity Lifecycle

        /// <summary>
        /// Called when the Activity is created.
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            this.SetContentView(Resource.Layout.Main);

            string path = Environment.ExternalStorageDirectory.AbsolutePath + "/test.mp4";

            var record = FindViewById<Button>(Resource.Id.Record);
            var stop = FindViewById<Button>(Resource.Id.Stop);
            var play = FindViewById<Button>(Resource.Id.Play);
            this.video = this.FindViewById<VideoView>(Resource.Id.SampleVideoView);

            // Use the device's front facing camera.
            this.camera = this.GetFirstFrontFacingCamera();
            this.camera.SetPreviewDisplay(this.video.Holder);
            this.camera.StartPreview();

            record.Click += delegate
            {
                this.video.StopPlayback();

                this.recorder = new MediaRecorder();
                this.camera.StopPreview();
                this.camera.Unlock();
                this.recorder.SetCamera(this.camera);

                this.recorder.SetOutputFormat(OutputFormat.Default);
                this.recorder.SetAudioSource(AudioSource.Default);
                this.recorder.SetVideoSource(VideoSource.Default);
                this.recorder.SetAudioEncoder(AudioEncoder.Default);
                this.recorder.SetVideoEncoder(VideoEncoder.Default);
                this.recorder.SetOutputFile(path);
                this.recorder.SetPreviewDisplay(this.video.Holder.Surface);
                this.recorder.Prepare();
                this.recorder.Start();
            };

            stop.Click += delegate
            {
                if (this.recorder != null)
                {
                    this.recorder.Stop();
                    this.recorder.Release();
                    this.camera.Lock();
                    this.camera.SetPreviewDisplay(this.video.Holder);
                    this.camera.StartPreview();
                }
            };

            play.Click += delegate
            {
                var uri = Android.Net.Uri.Parse(path);
                this.video.SetVideoURI(uri);
                this.video.Start();
            };
        }

        /// <summary>
        /// Called at the end of the Activity's lifecycle.
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (this.recorder != null)
            {
                this.recorder.Release();
                this.recorder.Dispose();
                this.recorder = null;
                this.camera.Release();
                this.camera = null;
            }
        }

        #endregion Android Activity Lifecycle

        /// <summary>
        /// Gets a reference to the device's first front facing camera.
        /// </summary>
        /// <returns>A reference to a Camera instance.</returns>
        private Camera GetFirstFrontFacingCamera()
        {
            Camera frontFacingCamera = null;
            int numberOfCameras = Camera.NumberOfCameras;

            for (int cameraId = 0; cameraId < numberOfCameras; cameraId++)
            {
                var cameraInfo = new Camera.CameraInfo();
                Camera.GetCameraInfo(cameraId, cameraInfo);

                if (cameraInfo.Facing == CameraFacing.Front)
                {
                    try
                    {
                        frontFacingCamera = Camera.Open(cameraId);
                    }
                    catch (Exception exception)
                    {
                        Log.Error("VideoCam", exception.Message);
                    }
                }
            }

            return frontFacingCamera;
        }
    }
}