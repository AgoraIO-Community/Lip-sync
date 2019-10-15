using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using AOT;

namespace agora_gaming_rtc
{
    public class AudioRawDataManager : IRtcEngineBase
    {
        private static IRtcEngine _irtcEngine;
        private static AudioRawDataManager _audioRawDataManagerInstance;
        
        public delegate void OnRecordAudioFrameHandler(AudioFrame audioFrame);
        private OnRecordAudioFrameHandler OnRecordAudioFrame;

        public delegate void OnPlaybackAudioFrameHandler(AudioFrame audioFrame);
        private OnPlaybackAudioFrameHandler OnPlaybackAudioFrame;

        public delegate void OnMixedAudioFrameHandler(AudioFrame audioFrame);
        private OnMixedAudioFrameHandler OnMixedAudioFrame;

        public delegate void OnPlaybackAudioFrameBeforeMixingHandler(uint uid, AudioFrame audioFrame);
        private OnPlaybackAudioFrameBeforeMixingHandler OnPlaybackAudioFrameBeforeMixing;

        private AudioRawDataManager(IRtcEngine irtcEngine)
        {
            _irtcEngine = irtcEngine;
        }

        public static AudioRawDataManager GetInstance(IRtcEngine irtcEngine)
        {
            if (irtcEngine == null)
                return null;

            if (_audioRawDataManagerInstance == null)
                _audioRawDataManagerInstance = new AudioRawDataManager(irtcEngine);

            return _audioRawDataManagerInstance;
        }

        public static void ReleaseInstance()
		{
			_audioRawDataManagerInstance = null;
		}

        public void SetEngine(IRtcEngine irtcEngine)
        {
            _irtcEngine = irtcEngine;
        }

        public int SetOnRecordAudioFrameCallback(OnRecordAudioFrameHandler action)
        {
            if (_irtcEngine == null)
                return -7;

            if (action == null)
            {
                OnRecordAudioFrame = null;
                initEventOnRecordAudioFrame(null);
            }
            else
            {
                OnRecordAudioFrame = action;
                initEventOnRecordAudioFrame(OnRecordAudioFrameCallback);
            }
            return 0;
        }

        public int SetOnPlaybackAudioFrameCallback(OnPlaybackAudioFrameHandler action)
        {
            if (_irtcEngine == null)
                return -7;

            if (action == null)
            {
                OnPlaybackAudioFrame = null;
                initEventOnPlaybackAudioFrame(null);
            }
            else
            {
                OnPlaybackAudioFrame = action;
                initEventOnPlaybackAudioFrame(OnPlaybackAudioFrameCallback);
            }
            return 0;
        }

        public int SetOnMixedAudioFrameCallback(OnMixedAudioFrameHandler action)
        {
            if (_irtcEngine == null)
                return -7;

            if (action == null)
            {
                OnMixedAudioFrame = null;
                initEventOnMixedAudioFrame(null);
            }
            else
            {
                OnMixedAudioFrame = action;
                initEventOnMixedAudioFrame(OnMixedAudioFrameCallback);
            }
            return 0;
        }

        public int SetOnPlaybackAudioFrameBeforeMixingCallback(OnPlaybackAudioFrameBeforeMixingHandler action)
        {
            if (_irtcEngine == null)
                return -7;

            if (action == null)
            {
                OnPlaybackAudioFrameBeforeMixing = null;
                initEventOnPlaybackAudioFrameBeforeMixing(null);
            }
            else
            {
                OnPlaybackAudioFrameBeforeMixing = action;
                initEventOnPlaybackAudioFrameBeforeMixing(OnPlaybackAudioFrameBeforeMixingCallback);
            }
            return 0;
        }

        public int RegisteAudioRawDataObserver()
        {
            if (_irtcEngine == null)
                return -7;

#if UNITY_IOS || UNITY_STANDALONE_OSX
            return registerAudioFrameObserver_();
#elif UNITY_STANDALONE_WIN || UNITY_ANDROID
            return registerAudioFrameObserver();
#endif
        }

        public int UnRegisteAudioRawDataObserver()
        {
            if (_irtcEngine == null)
                return -7;

#if UNITY_IOS || UNITY_STANDALONE_OSX
            return unRegisterAudioFrameObserver_();
#elif UNITY_STANDALONE_WIN || UNITY_ANDROID
            return unRegisterAudioFrameObserver();
#endif
        }
        
        [MonoPInvokeCallback(typeof(EngineEventOnRecordAudioFrame))]
        private static void OnRecordAudioFrameCallback(int type, int samples, int bytesPerSample, int channels, int samplesPerSec, IntPtr buffer, Int64 renderTimeMs, int avsync_type)
        {
            if (_irtcEngine != null && _audioRawDataManagerInstance != null && _audioRawDataManagerInstance.OnRecordAudioFrame != null)
            {
                AudioFrame audioFrame = new AudioFrame();
                audioFrame.type = (AUDIO_FRAME_TYPE)type;
                audioFrame.samples = samples;
                audioFrame.bytesPerSample = bytesPerSample;
                audioFrame.channels = channels;
                audioFrame.samplesPerSec = samplesPerSec;
                byte[] byteBuffer = new byte[bytesPerSample * channels * samples];
                Marshal.Copy(buffer, byteBuffer, 0, bytesPerSample * channels * samples);
                audioFrame.buffer = byteBuffer;
                audioFrame.renderTimeMs = renderTimeMs;
                audioFrame.avsync_type = avsync_type;
                _audioRawDataManagerInstance.OnRecordAudioFrame(audioFrame);
            }
        } 

        [MonoPInvokeCallback(typeof(EngineEventOnPlaybackAudioFrame))]
        private static void OnPlaybackAudioFrameCallback(int type, int samples, int bytesPerSample, int channels, int samplesPerSec, IntPtr buffer, Int64 renderTimeMs, int avsync_type)
        {
            if (_irtcEngine != null && _audioRawDataManagerInstance != null && _audioRawDataManagerInstance.OnPlaybackAudioFrame != null)
            {
                AudioFrame audioFrame = new AudioFrame();
                audioFrame.type = (AUDIO_FRAME_TYPE)type;
                audioFrame.samples = samples;
                audioFrame.bytesPerSample = bytesPerSample;
                audioFrame.channels = channels;
                audioFrame.samplesPerSec = samplesPerSec;
                byte[] byteBuffer = new byte[bytesPerSample * channels * samples];
                Marshal.Copy(buffer, byteBuffer, 0, bytesPerSample * channels * samples);
                audioFrame.buffer = byteBuffer;
                audioFrame.renderTimeMs = renderTimeMs;
                audioFrame.avsync_type = avsync_type;
                _audioRawDataManagerInstance.OnPlaybackAudioFrame(audioFrame);
            }
        } 

        [MonoPInvokeCallback(typeof(EngineEventOnMixedAudioFrame))]
        private static void OnMixedAudioFrameCallback(int type, int samples, int bytesPerSample, int channels, int samplesPerSec, IntPtr buffer, Int64 renderTimeMs, int avsync_type)
        {
            if (_irtcEngine != null && _audioRawDataManagerInstance != null && _audioRawDataManagerInstance.OnMixedAudioFrame != null)
            {
                AudioFrame audioFrame = new AudioFrame();
                audioFrame.type = (AUDIO_FRAME_TYPE)type;
                audioFrame.samples = samples;
                audioFrame.bytesPerSample = bytesPerSample;
                audioFrame.channels = channels;
                audioFrame.samplesPerSec = samplesPerSec;
                byte[] byteBuffer = new byte[bytesPerSample * channels * samples];
                Marshal.Copy(buffer, byteBuffer, 0, bytesPerSample * channels * samples);
                audioFrame.buffer = byteBuffer;
                audioFrame.renderTimeMs = renderTimeMs;
                audioFrame.avsync_type = avsync_type;
                _audioRawDataManagerInstance.OnMixedAudioFrame(audioFrame);
            }
        } 

        [MonoPInvokeCallback(typeof(EngineEventOnPlaybackAudioFrameBeforeMixing))]
        private static void OnPlaybackAudioFrameBeforeMixingCallback(uint uid, int type, int samples, int bytesPerSample, int channels, int samplesPerSec, IntPtr buffer, Int64 renderTimeMs, int avsync_type)
        {
            if (_irtcEngine != null && _audioRawDataManagerInstance != null && _audioRawDataManagerInstance.OnPlaybackAudioFrameBeforeMixing != null)
            {
                AudioFrame audioFrame = new AudioFrame();
                audioFrame.type = (AUDIO_FRAME_TYPE)type;
                audioFrame.samples = samples;
                audioFrame.bytesPerSample = bytesPerSample;
                audioFrame.channels = channels;
                audioFrame.samplesPerSec = samplesPerSec;
                byte[] byteBuffer = new byte[bytesPerSample * channels * samples];
                Marshal.Copy(buffer, byteBuffer, 0, bytesPerSample * channels * samples);
                audioFrame.buffer = byteBuffer;
                audioFrame.renderTimeMs = renderTimeMs;
                audioFrame.avsync_type = avsync_type;
                _audioRawDataManagerInstance.OnPlaybackAudioFrameBeforeMixing(uid, audioFrame);
            }
        } 
    }
}