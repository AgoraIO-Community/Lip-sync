using UnityEngine;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using AOT;

namespace agora_gaming_rtc
{
    #region some enum types
    public struct RtcStats
    {
        public uint duration;
        public uint txBytes;
        public uint rxBytes;
        public ushort txKBitRate;
        public ushort rxKBitRate;
        public ushort txAudioKBitRate;
        public ushort rxAudioKBitRate;
        public ushort txVideoKBitRate;
        public ushort rxVideoKBitRate;
        public ushort lastmileQuality;
        public uint users;
        public double cpuAppUsage;
        public double cpuTotalUsage;
        public ushort txPacketLossRate;
        public ushort rxPacketLossRate;
    };

    public enum USER_OFFLINE_REASON
    {
        QUIT = 0,
        DROPPED = 1,
        BECOME_AUDIENCE = 2,
    };

    public struct AudioVolumeInfo
    {
        public uint uid;
        public uint volume; // [0, 255]
    };

    public enum LOG_FILTER
    {
        OFF = 0,
        DEBUG = 0x80f,
        INFO = 0x0f,
        WARNING = 0x0e,
        ERROR = 0x0c,
        CRITICAL = 0x08,
    };

    public enum CHANNEL_PROFILE
    {
        GAME_FREE_MODE = 0,
        GAME_COMMAND_MODE = 1,
    };

    public enum CLIENT_ROLE
    {
        BROADCASTER = 1,
        AUDIENCE = 2,
    };

    public enum AUDIO_RECORDING_QUALITY_TYPE
    {
        AUDIO_RECORDING_QUALITY_LOW = 0,
        AUDIO_RECORDING_QUALITY_MEDIUM = 1,
        AUDIO_RECORDING_QUALITY_HIGH = 2,
    };

    public enum AUDIO_ROUTE
    {
        DEFAULT = -1,
        HEADSET = 0,
        EARPIECE = 1,
        SPEAKERPHONE = 3,
        BLUETOOTH = 5,
    };

    public struct ExternalVideoFrame
    {
        public enum VIDEO_BUFFER_TYPE
        {
            VIDEO_BUFFER_RAW_DATA = 1,
        };

        public enum VIDEO_PIXEL_FORMAT
        {
            VIDEO_PIXEL_UNKNOWN = 0,
            VIDEO_PIXEL_I420 = 1,
            VIDEO_PIXEL_BGRA = 2,
            VIDEO_PIXEL_NV12 = 8,
        };

        public VIDEO_BUFFER_TYPE type;
        public VIDEO_PIXEL_FORMAT format;
        public byte[] buffer;
        public int stride;
        public int height;
        public int cropLeft;
        public int cropTop;
        public int cropRight;
        public int cropBottom;
        public int rotation;
        public long timestamp;
    };
     public struct TranscodingUser
    {
        /** User ID of the user displaying the video in the CDN live.
        */
        public uint uid;

        /** Horizontal position from the top left corner of the video frame.
*/
        public int x;
        /** Vertical position from the top left corner of the video frame.
        */
        public int y;
        /** Width of the video frame. The default value is 360.
        */
        public int width;
        /** Height of the video frame. The default value is 640.
        */
        public int height;

        /** Layer position of the video frame. The value ranges between 0 and 100.

         - 0: (Default) Lowest
         - 100: Highest

         @note
         - If zOrder is beyond this range, the SDK reports #ERR_INVALID_ARGUMENT.
         - As of v2.3, the SDK supports zOrder = 0.
         */
        public int zOrder;
        /**  Transparency of the video frame in CDN live. The value ranges between 0 and 1.0:

         - 0: Completely transparent
         - 1.0: (Default) Opaque
         */
        public double alpha;
        /** The audio channel of the sound. The default value is 0:

         - 0: (Default) Supports dual channels at most, depending on the upstream of the broadcaster.
         - 1: The audio stream of the broadcaster uses the FL audio channel. If the upstream of the broadcaster uses multiple audio channels, these channels will be mixed into mono first.
         - 2: The audio stream of the broadcaster uses the FC audio channel. If the upstream of the broadcaster uses multiple audio channels, these channels will be mixed into mono first.
         - 3: The audio stream of the broadcaster uses the FR audio channel. If the upstream of the broadcaster uses multiple audio channels, these channels will be mixed into mono first.
         - 4: The audio stream of the broadcaster uses the BL audio channel. If the upstream of the broadcaster uses multiple audio channels, these channels will be mixed into mono first.
         - 5: The audio stream of the broadcaster uses the BR audio channel. If the upstream of the broadcaster uses multiple audio channels, these channels will be mixed into mono first.

         @note If your setting is not 0, you may need a specialized player.
         */
        public int audioChannel;
    };

    public struct RtcImage
    {
        /** URL address of the image on the broadcasting video. */
        public string url;
        /** Horizontal position of the image from the upper left of the broadcasting video. */
        public int x;
        /** Vertical position of the image from the upper left of the broadcasting video. */
        public int y;
        /** Width of the image on the broadcasting video. */
        public int width;
        /** Height of the image on the broadcasting video. */
        public int height;
    }

    public enum VIDEO_CODEC_PROFILE_TYPE
    {  /** 66: Baseline video codec profile. Generally used in video calls on mobile phones. */
        VIDEO_CODEC_PROFILE_BASELINE = 66,
        /** 77: Main video codec profile. Generally used in mainstream electronics such as MP4 players, portable video players, PSP, and iPads. */
        VIDEO_CODEC_PROFILE_MAIN = 77,
        /**  100: (Default) High video codec profile. Generally used in high-resolution broadcasts or television. */
        VIDEO_CODEC_PROFILE_HIGH = 100,
    };

    public enum AUDIO_SAMPLE_RATE_TYPE
    {
        /** 32000: 32 kHz */
        AUDIO_SAMPLE_RATE_32000 = 32000,
        /** 44100: 44.1 kHz */
        AUDIO_SAMPLE_RATE_44100 = 44100,
        /** 48000: 48 kHz */
        AUDIO_SAMPLE_RATE_48000 = 48000,
    };

    public enum AUDIO_CODEC_PROFILE_TYPE
    {
        AUDIO_CODEC_PROFILE_LC_AAC = 0,
        AUDIO_CODEC_PROFILE_HE_AAC = 1,
    };

    public struct LiveTranscoding
    {
        /** Width of the video. The default value is 360.
         */
        public int width;
        /** Height of the video. The default value is 640.
        */
        public int height;
        /** bit rate of the CDN live output video stream. The default value is 400 K bps.
        */
        public int videoBitrate;
        /** Frame rate of the output video stream set for the CDN live broadcast. The default value is 15 fps.
        */
        public int videoFramerate;

        /** Latency mode:

         - true: Low latency with u n assured quality.
         - false: (Default) High latency with assured quality.
         */
        public bool lowLatency;

        /** Video GOP in frames. The default value is 30 fps.
        */
        public int videoGop;
        /** Self-defined video codec profile: #VIDEO_CODEC_PROFILE_TYPE
        */
        public VIDEO_CODEC_PROFILE_TYPE videoCodecProfile;
        /** RGB hex value.

         Background color of the output video stream for the CDN live broadcast defined as int color = (A & 0xff) << 24 | (R & 0xff) << 16 | (G & 0xff) << 8 | (B & 0xff). Users can set the *backgroundColor* with ARGB ColorInt.

         @note Value only, do not include a #. For example, C0C0C0.
         */
        public uint backgroundColor;
        /** The number of users in the live broadcast.
         */
        public uint userCount;
        /** TranscodingUser
        */
        public TranscodingUser transcodingUsers;
        /** Reserved property. Extra user-defined information to send SEI for the H.264/H.265 video stream to the CDN live client.
         */
        public string transcodingExtraInfo;

        /** The metadata sent to the CDN live client defined by the RTMP or FLV metadata.
         */
        public string metadata;
        /** The watermark image added to the CDN live publishing stream.

        The audience of the CDN live publishing stream can see the watermark image. See RtcImage.
        */
        public RtcImage watermark;
        /** The background image added to the CDN live publishing stream.

        The audience of the CDN live publishing stream can see the background image. See RtcImage.
        */
        public RtcImage backgroundImage;
        /** Self-defined audio-sampling rate: #AUDIO_SAMPLE_RATE_TYPE.
        */
        public AUDIO_SAMPLE_RATE_TYPE audioSampleRate;
        /** bit rate of the CDN live audio output stream. The default value is 48 K bps, and the highest value is 128.
         */
        public int audioBitrate;
        /** agora 's self-defined audio-channel types. agora recommends choosing option 1 or 2. A special player is required if you choose option 3, 4, or 5:

         - 1: (Default) Mono
         - 2: Two-channel stereo
         - 3: Three-channel stereo
         - 4: Four-channel stereo
         - 5: Five-channel stereo
         */
        public int audioChannels;
        public AUDIO_CODEC_PROFILE_TYPE audioCodecProfile;
    };

    /** **DEPRECATED**
    */
    public struct PublisherConfiguration {
    /** Width of the CDN live output video stream. The default value is 360.
    */
        public int width;
    /** Height of the CDN live output video stream. The default value is 640.
    */
        public int height;
    /** Frame rate of the CDN live output video stream. The default value is 15 fps.
    */
        public int framerate;
    /** Bitrate of the CDN live output video stream. The default value is 500 Kbps.
    */
        public int bitrate;
    /** Default layout:

    - 0: Tile horizontally
    - 1: Layered windows
    - 2: Tile vertically
    */
        public int defaultLayout;
        /** Video stream lifecycle of CDN live: #RTMP_STREAM_LIFE_CYCLE_TYPE.
    */
        public int lifecycle;
    /** Whether or not the current user is the owner of the RTMP stream:

    - true: (Default) Yes. The push-stream configuration takes effect.
    - false: No. The push-stream configuration does not work.
    */
        public bool owner;
    /** Width of the injected stream. N/A. Set it as 0.
    */
        public int injectStreamWidth;
    /** Height of the injected stream. N/A. Set it as 0.
    */
        public int injectStreamHeight;
    /** URL address of the injected stream in the channel. N/A.
    */
        public string injectStreamUrl;
    /** Push-stream URL address for the picture-in-picture layout. The default value is NULL.
    */
        public string publishUrl;
    /** The push-stream URL address of the original stream that does not require picture-blending. The default value is NULL.
    */
        public string rawStreamUrl;
    /** Reserved field. The default value is NULL.
    */
        public string extraInfo;
    };


    /** Video display modes. */
    public enum RENDER_MODE_TYPE
    {
    /**
    1: Uniformly scale the video until it fills the visible boundaries (cropped). One dimension of the video may have clipped contents.
    */
        RENDER_MODE_HIDDEN = 1,
        /**
    2: Uniformly scale the video until one of its dimension fits the boundary (zoomed to fit). Areas that are not filled due to disparity in the aspect ratio are filled with black.
    */
        RENDER_MODE_FIT = 2,
        /** **DEPRECATED** 3: This mode is deprecated.
        */
        RENDER_MODE_ADAPTIVE = 3,
    };
    /** **DEPRECATED** RTC video compositing layout.
    */
    public struct VideoCompositingLayout
    {
    /** **DEPRECATED** The display region of a specified user on the screen.
    */
        public struct Region {
        /** User ID of the user whose video is displayed on the screen.
        */
            public uint uid;
            /** Horizontal position of the region on the screen. The value ranges between 0.0 and 1.0.
            */
            public double x;//[0,1]
            /** Vertical position of the region on the screen. The value ranges between 0.0 and 1.0.
            */
            public double y;//[0,1]
            /** Actual width of the region. The value ranges between 0.0 and 1.0. For example, if the width of the screen is 360 and the width of the region is 120, set the width value as 0.33.
            */
            public double width;//[0,1]
            /** Actual height of the region. The value ranges between 0.0 and 1.0. For example, if the height of the screen is 240 and the height of the region is 120, set the height value as 0.5.
            */
            public double height;//[0,1]
            /** Layer position of the region:

            - 0: (Default) The region is at the bottom layer.
            - 100: The region is at the top layer.

            @note
            - If zOrder is beyond this range, the SDK reports #ERR_INVALID_ARGUMENT.
            - As of v2.3, the SDK supports zOrder = 0.
            */
            public int zOrder; //optional, [0, 100] //0 (default): bottom most, 100: top most

        /** The transparency of the video region in CDN live. The value ranges between 0.0 and 1.0:

        - 0: The region is transparent.
        - 1: (Default) The region is opaque.
        */
            public double alpha;
            /** **DEPRECATED** This parameter does not take effect. You can ignore it.
            */
            public RENDER_MODE_TYPE renderMode;

        };
        /** Ignore this parameter. The width of the canvas is set by the \ref IRtcEngine::configPublisher "configPublisher" method instead.
        */
        public int canvasWidth;
        /** Ignore this parameter. The height of the canvas is set by the \ref IRtcEngine::configPublisher "configPublisher" method instead.
        */
        public int canvasHeight;
        /** RGB hex color value.

        @note Value only, do not include a #. For example, C0C0C0.
        */
        public string backgroundColor;
        /** Screen display region information.

        Sets the screen display region of a host or a delegated host in CDN live streaming. See Region for more information.
        */
        public Region regions;
        /** Number of the screen display regions.
        */
        public int regionCount;
        /** Application-defined data. Supports a maximum of 2048 bytes.
        */
        public string appData;
        /** Length of the user-defined application data.
        */
        public int appDataLength;  
    };

    public enum LIGHTENING_CONTRAST_LEVEL
    {
        /** Low contrast level. */
        LIGHTENING_CONTRAST_LOW = 0,
        /** (Default) Normal contrast level. */
        LIGHTENING_CONTRAST_NORMAL,
        /** High contrast level. */
        LIGHTENING_CONTRAST_HIGH
    };

    /** Image enhancement options.
    */
    public struct BeautyOptions {
    
        /** The contrast level, used with the @p lightening parameter.
        */
        public LIGHTENING_CONTRAST_LEVEL lighteningContrastLevel;
            
        /** The brightness level. The value ranges from 0.0 (original) to 1.0. */
        public float lighteningLevel;
            
        /** The sharpness level. The value ranges between 0 (original) and 1. This parameter is usually used to remove blemishes.
        */
        public float smoothnessLevel;
            
        /** The redness level. The value ranges between 0 (original) and 1. This parameter adjusts the red saturation level. 
        */
        public float rednessLevel;
    };


    /** Configuration of the imported live broadcast voice or video stream.
    */
    public struct InjectStreamConfig {
        /** Width of the added stream in the live broadcast. The default value is 0 (same width as the original stream).
        */
        public int width;
        /** Height of the added stream in the live broadcast. The default value is 0 (same height as the original stream).
        */
        public int height;
        /** Video GOP of the added stream in the live broadcast in frames. The default value is 30 fps.
        */
        public int videoGop;
        /** Video frame rate of the added stream in the live broadcast. The default value is 15 fps.
        */
        public int videoFramerate;
        /** Video bitrate of the added stream in the live broadcast. The default value is 400 Kbps.

        @note The setting of the video bitrate is closely linked to the resolution. If the video bitrate you set is beyond a reasonable range, the SDK sets it within a reasonable range.
        */
        public int videoBitrate;
        /** Audio-sample rate of the added stream in the live broadcast: #AUDIO_SAMPLE_RATE_TYPE. The default value is 48000 Hz.

        @note We recommend setting the default value.
        */
        public AUDIO_SAMPLE_RATE_TYPE audioSampleRate;
        /** Audio bitrate of the added stream in the live broadcast. The default value is 48.

        @note We recommend setting the default value.
        */
        public int audioBitrate;
        /** Audio channels in the live broadcast.

        - 1: (Default) Mono
        - 2: Two-channel stereo

        @note We recommend setting the default value.
        */
        public int audioChannels;
    };

    public enum AUDIO_FRAME_TYPE {
        FRAME_TYPE_PCM16 = 0,  //PCM 16bit little endian
    };

    public struct AudioFrame {
        public AUDIO_FRAME_TYPE type;
        public int samples;  //number of samples in this frame
        public int bytesPerSample;  //number of bytes per sample: 2 for PCM16
        public int channels;  //number of channels (data are interleaved if stereo)
        public int samplesPerSec;  //sampling rate
        public byte[] buffer;  //data buffer
        public Int64 renderTimeMs;
        public int avsync_type;
    };


    #endregion some enum types

    public abstract class IRtcEngineBase
    {
        #region DllImport
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		public const string MyLibName = "agoraSdkCWrapper";
#else

#if UNITY_IPHONE
		public const string MyLibName = "__Internal";
#else
        public const string MyLibName = "agoraSdkCWrapper";
#endif
#endif

        protected const string agoraGameObjectName = "agora_engine_CallBackGamObject";
        protected static GameObject agoraGameObject = null;

        // standard sdk api
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern bool createEngine(string appId);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void deleteEngine();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern IntPtr getSdkVersion();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int joinChannel(string channelKey, string channelName, string info, uint uid);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setLocalVoicePitch(double pitch);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setRemoteVoicePosition(uint uid, double pan, double gain);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setVoiceOnlyMode(bool enable);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int leaveChannel();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int enableLastmileTest();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int disableLastmileTest();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int enableVideo();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int disableVideo();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int enableLocalVideo(bool enabled);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int enableLocalAudio(bool enabled);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int startPreview();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int stopPreview();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int enableAudio();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int disableAudio();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setParameters(string options);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern IntPtr getCallId();
        // caller free the returned char * (through freeObject)
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int rate(string callId, int rating, string desc);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int complain(string callId, string desc);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setEnableSpeakerphone(bool enabled);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int isSpeakerphoneEnabled();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setDefaultAudioRoutetoSpeakerphone(bool enabled);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int enableAudioVolumeIndication(int interval, int smooth);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int startAudioRecording(string filePath, int quality);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int stopAudioRecording();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int startAudioMixing(string filePath, bool loopBack, bool replace, int cycle, int playTime);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int stopAudioMixing();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int pauseAudioMixing();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int resumeAudioMixing();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int adjustAudioMixingVolume(int volume);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int getAudioMixingDuration();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int getAudioMixingCurrentPosition();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int muteLocalAudioStream(bool mute);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int muteAllRemoteAudioStreams(bool mute);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int muteRemoteAudioStream(uint uid, bool mute);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int switchCamera();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setVideoProfile(int profile, bool swapWidthAndHeight);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int muteLocalVideoStream(bool mute);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int muteAllRemoteVideoStreams(bool mute);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int muteRemoteVideoStream(uint uid, bool mute);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setLogFile(string filePath);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int renewToken(string token);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setChannelProfile(int profile);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setClientRole(int role);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int enableDualStreamMode(bool enabled);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setEncryptionMode(string encryptionMode);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setEncryptionSecret(string secret);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int startRecordingService(string recordingKey);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int stopRecordingService(string recordingKey);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int refreshRecordingServiceStatus();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int createDataStream(bool reliable, bool ordered);
        // TODO! supports general data later. now only string is supported
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int sendStreamMessage(int streamId, string data, Int64 length);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setRecordingAudioFrameParametersWithSampleRate(int sampleRate, int channel, int mode, int samplesPerCall);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setPlaybackAudioFrameParametersWithSampleRate(int sampleRate, int channel, int mode, int samplesPerCall);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setSpeakerphoneVolume(int volume);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int adjustRecordingSignalVolume(int volume);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int adjustPlaybackSignalVolume(int volume);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setHighQualityAudioParametersWithFullband(int fullband, int stereo, int fullBitrate);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int enableInEarMonitoring(bool enabled);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int enableWebSdkInteroperability(bool enabled);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setVideoQualityParameters(bool preferFrameRateOverImageQuality);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int startEchoTest();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int stopEchoTest();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setRemoteVideoStreamType(uint uid, int streamType);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setMixedAudioFrameParameters(int sampleRate, int samplesPerCall);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setAudioMixingPosition(int pos);
        // setLogFilter: deprecated
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setLogFilter(uint filter);
        // video texture stuff (extension for gaming)
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int enableVideoObserver();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int disableVideoObserver();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int generateNativeTexture();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int updateTexture(int tex, IntPtr data, uint uid);
        // return value: -1 for no update; otherwise (width << 16 | height)
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void deleteTexture(int tex);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setPlaybackDeviceVolume(int volume);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int getEffectsVolume();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setEffectsVolume(int volume);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int playEffect(int soundId, string filePath, int loopCount, double pitch, double pan, int gain, bool publish);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int stopEffect(int soundId);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int stopAllEffects();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int preloadEffect(int soundId, string filePath);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int unloadEffect(int soundId);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int pauseEffect(int soundId);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int pauseAllEffects();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int resumeEffect(int soundId);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int resumeAllEffects();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setDefaultMuteAllRemoteAudioStreams(bool mute);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setDefaultMuteAllRemoteVideoStreams(bool mute);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int pushExternVideoFrame(int type, int format, byte[] buffer, int stride, int height, int cropLeft, int cropTop, int cropRight, int cropBottom, int rotation, long timestamp);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int setExternalVideoSource(bool enable, bool useTexture);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int setLiveTranscoding(int width, int height, int videoBitrate, int videoFramerate, bool lowLatency, int videoGroup, int video_codec_profile, uint backgroundColor, uint userCount, uint transcodingUserUid, int transcodingUsersX, int transcodingUsersY, int transcodingUsersWidth, int transcodingUsersHeight, int transcodingUsersZorder, double transcodingUsersAlpha, int transcodingUsersAudioChannel, string transcodingExtraInfo, string metaData, string watermarkRtcImageUrl, int watermarkRtcImageX, int watermarkRtcImageY, int watermarkRtcImageWidth, int watermarkRtcImageHeight, string backgroundImageRtcImageUrl, int backgroundImageRtcImageX, int backgroundImageRtcImageY, int backgroundImageRtcImageWidth, int backgroundImageRtcImageHeight, int audioSampleRate, int audioBitrate, int audioChannels, int audioCodecProfile);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern  int addPublishStreamUrl(string url, bool transcodingEnabled);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int removePublishStreamUrl(string url);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int configPublisher(int width, int height, int framerate, int bitrate, int defaultLayout, int lifecycle, bool owner, int injectStreamWidth, int injectStreamHeight, string injectStreamUrl, string publishUrl, string rawStreamUrl, string extraInfo);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int setVideoCompositingLayout(int canvasWidth, int canvasHeight, string backgroundColor, uint regionUid, double regionX, double regionY, double regionWidth, double regionHeight, int regionZOrder, double regionAlpha, int regionRenderMode, int regionCount, string appData, int appDataLength);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int clearVideoCompositingLayout();
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int addVideoWatermark(string url, int x, int y, int width, int height);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int clearVideoWatermarks();
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int setBeautyEffectOptions(bool enabled, int beatyOptionsLighteningContrastLevel, float beatyOptionsLighteningLevel, float beatyOptionsSmoothnessLevel, float beatyOptionsRednessLevel);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int addInjectStreamUrl(string url, int injectStreamWidth, int injectStreamHeight, int injectStreamVideoGop, int injectStreamVideoFramerate, int injectStreamVideoBitrate, int injectStreamAudioSampleRate, int injectStreamAudioBitrate, int injectStreamAudioChannels);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int removeInjectStreamUrl(string url);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int enableSoundPositionIndication(bool enabled);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int setExternalAudioSource(bool enabled, int sampleRate, int channels);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int setExternalAudioSink(bool enabled, int sampleRate, int channels);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int pushAudioFrame_(int type, int samples, int bytesPerSample, int channels, int samplesPerSec, byte[] buffer, Int64 renderTimeMs, int avsync_type);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        public static extern int pushAudioFrame(int type, int samples, int bytesPerSample, int channels, int samplesPerSec, byte[] buffer, Int64 renderTimeMs, int avsync_type);


        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void freeObject(IntPtr obj);

        protected delegate void EngineEventOnJoinChannelSuccessHandler(string channel, uint uid, int elapsed);

        protected delegate void EngineEventOnLeaveChannelHandler(uint duration, uint txBytes, uint rxBytes,
                                    ushort txKBitRate, ushort rxKBitRate, ushort rxAudioKBitRate,
                                    ushort txAudioKBitRate, ushort rxVideoKBitRate,
                                    ushort txVideoKBitRate, ushort lastmileQuality, ushort txPacketLossRate, ushort rxPacketLossRate, uint userCount, double cpuAppUsage,
                                    double cpuTotalUsage);

        protected delegate void EngineEventOnReJoinChannelSuccessHandler(string channel, uint uid, int elapsed);

        protected delegate void EngineEventOnConnectionLostHandler();

        protected delegate void EngineEventOnConnectionInterruptedHandler();

        protected delegate void EngineEventOnRequestTokenHandler();

        protected delegate void EngineEventOnUserJoinedHandler(uint uid, int elapsed);

        protected delegate void EngineEventOnUserOfflineHandler(uint uid, int offLineReason);

        protected delegate void EngineEventOnAudioVolumeIndicationHandler(string volumeInfo, int speakerNumber, int totalVolume);

        protected delegate void EngineEventOnUserMuteAudioHandler(uint uid, bool muted);

        protected delegate void EngineEventOnSDKWarningHandler(int warn, string msg);

        protected delegate void EngineEventOnSDKErrorHandler(int error, string msg);

        protected delegate void EngineEventOnRtcStatsHandler(uint duration, uint txBytes, uint rxBytes,
                                    ushort txKBitRate, ushort rxKBitRate, ushort rxAudioKBitRate,
                                    ushort txAudioKBitRate, ushort rxVideoKBitRate,
                                    ushort txVideoKBitRate, ushort lastmileQuality, ushort txPacketLossRate, ushort rxPacketLossRate, uint userCount, double cpuAppUsage,
                                    double cpuTotalUsage);

        protected delegate void EngineEventOnAudioMixingFinishedHandler();

        protected delegate void EngineEventOnAudioRouteChangedHandler(int route);

        protected delegate void EngineEventOnFirstRemoteVideoDecodedHandler(uint uid, int width, int height, int elapsed);

        protected delegate void EngineEventOnVideoSizeChangedHandler(uint uid, int width, int height, int elapsed);

        protected delegate void EngineEventOnClientRoleChangedHandler(int oldRole, int newRole);

        protected delegate void EngineEventOnUserMuteVideoHandler(uint uid, bool muted);

        protected delegate void EngineEventOnMicrophoneEnabledHandler(bool isEnabled);

        protected delegate void EngineEventOnApiExecutedHandler(int err, string api, string result);

        protected delegate void EngineEventOnLastmileQualityHandler(int quality);

        protected delegate void EngineEventOnFirstLocalAudioFrameHandler(int elapsed);

        protected delegate void EngineEventOnFirstRemoteAudioFrameHandler(uint userId, int elapsed);

        protected delegate void EngineEventOnAudioQualityHandler(uint userId, int quality, ushort delay, ushort lost);

        protected delegate void EngineEventOnStreamInjectedStatusHandler(string url, uint userId, int status);

        protected delegate void EngineEventOnStreamUnpublishedHandler(string url);

        protected delegate void EngineEventOnStreamPublishedHandler(string url, int error);

        protected delegate void EngineEventOnStreamMessageErrorHandler(uint userId, int streamId, int code, int missed, int cached);

        protected delegate void EngineEventOnStreamMessageHandler(uint userId, int streamId, string data, int length);

        protected delegate void EngineEventOnConnectionBannedHandler();

        protected delegate void EngineEventOnNetworkQualityHandler(uint uid, int txQuality, int rxQuality);

        protected delegate void EngineEventOnRecordAudioFrame(int type, int samples, int bytesPerSample, int channels, int samplesPerSec, IntPtr buffer, Int64 renderTimeMs, int avsync_type);

        protected delegate void EngineEventOnPlaybackAudioFrame(int type, int samples, int bytesPerSample, int channels, int samplesPerSec, IntPtr buffer, Int64 renderTimeMs, int avsync_type);

        protected delegate void EngineEventOnMixedAudioFrame(int type, int samples, int bytesPerSample, int channels, int samplesPerSec, IntPtr buffer, Int64 renderTimeMs, int avsync_type);

        protected delegate void EngineEventOnPlaybackAudioFrameBeforeMixing(uint uid, int type, int samples, int bytesPerSample, int channels, int samplesPerSec, IntPtr buffer, Int64 renderTimeMs, int avsync_type);

        protected delegate void OnEngineEventHandler(int methodNumber, string data);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initCallBackObject();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnJoinChannelSuccess(EngineEventOnJoinChannelSuccessHandler OnJoinChannelSuccess);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnLeaveChannel(EngineEventOnLeaveChannelHandler OnLeaveChannel);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnReJoinChannelSuccess(EngineEventOnReJoinChannelSuccessHandler OnReJoinChannelSuccess);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnConnectionLost(EngineEventOnConnectionLostHandler OnConnectionLost);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnConnectionInterrupted(EngineEventOnConnectionInterruptedHandler OnConnectionInterrupted);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnRequestToken(EngineEventOnRequestTokenHandler OnRequestToken);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnUserJoined(EngineEventOnUserJoinedHandler OnUserJoined);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnUserOffline(EngineEventOnUserOfflineHandler OnUserOffline);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnAudioVolumeIndication(EngineEventOnAudioVolumeIndicationHandler OnAudioVolumeIndication);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnUserMuteAudio(EngineEventOnUserMuteAudioHandler OnUserMuteAudio);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnSDKWarning(EngineEventOnSDKWarningHandler OnSDKWarning);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnSDKError(EngineEventOnSDKErrorHandler OnSDKError);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnRtcStats(EngineEventOnRtcStatsHandler OnRtcStats);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnAudioMixingFinished(EngineEventOnAudioMixingFinishedHandler OnAudioMixingFinished);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnAudioRouteChanged(EngineEventOnAudioRouteChangedHandler OnAudioRouteChanged);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnFirstRemoteVideoDecoded(EngineEventOnFirstRemoteVideoDecodedHandler OnFirstRemoteVideoDecoded);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnVideoSizeChanged(EngineEventOnVideoSizeChangedHandler OnVideoSizeChanged);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnClientRoleChanged(EngineEventOnClientRoleChangedHandler OnVideoSizeChanged);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnUserMuteVideo(EngineEventOnUserMuteVideoHandler OnUserMuteVideo);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnMicrophoneEnabled(EngineEventOnMicrophoneEnabledHandler OnMicrophoneEnabled);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnApiExecuted(EngineEventOnApiExecutedHandler OnApiExecuted);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnFirstLocalAudioFrame(EngineEventOnFirstLocalAudioFrameHandler OnFirstLocalAudioFrame);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnFirstRemoteAudioFrame(EngineEventOnFirstRemoteAudioFrameHandler OnFirstRemoteAudioFrame);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnLastmileQuality(EngineEventOnLastmileQualityHandler OnLastmileQuality);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnAudioQuality(EngineEventOnAudioQualityHandler onAudioQuality);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnStreamInjectedStatus(EngineEventOnStreamInjectedStatusHandler onStreamInjectedStatus);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnStreamUnpublished(EngineEventOnStreamUnpublishedHandler onStreamUnpublished);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnStreamPublished(EngineEventOnStreamPublishedHandler onStreamPublished);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnStreamMessageError(EngineEventOnStreamMessageErrorHandler onStreamMessageError);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnStreamMessage(EngineEventOnStreamMessageHandler onStreamMessage);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnConnectionBanned(EngineEventOnConnectionBannedHandler onConnectionBanned);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnNetworkQuality(EngineEventOnNetworkQualityHandler onNetworkQuality);
        
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int registerAudioFrameObserver_();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int registerAudioFrameObserver();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int unRegisterAudioFrameObserver_();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern int unRegisterAudioFrameObserver();

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnRecordAudioFrame(EngineEventOnRecordAudioFrame onRecordAudioFrame);
    
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnPlaybackAudioFrame(EngineEventOnPlaybackAudioFrame onPlaybackAudioFrame);

        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnMixedAudioFrame(EngineEventOnMixedAudioFrame onMixedAudioFrame);
    
        [DllImport(MyLibName, CharSet = CharSet.Ansi)]
        protected static extern void initEventOnPlaybackAudioFrameBeforeMixing(EngineEventOnPlaybackAudioFrameBeforeMixing onPlaybackAudioFrameBeforeMixing);

        #endregion engine callbacks   
    }
}