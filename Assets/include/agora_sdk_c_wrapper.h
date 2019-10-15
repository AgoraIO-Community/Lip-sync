#ifndef AGORA_SDK_C_WRAPPER
#define AGORA_SDK_C_WRAPPER
#include "AgoraSDKObject.h"
#include <string>
#define NOT_INIT_AGORA_SDK_OBJECT -7
#define NOT_INIT_AGORA_SDK_OBJECT_CHAR "-7"
using namespace std;

const char *TAG = "agora_sdk_c_wrapper";
//----------------Agora Api Start-----------------------------
AGORA_API void createEngine(const char *appId);
AGORA_API void deleteEngine();
AGORA_API const char *getSdkVersion();
AGORA_API const char *getMediaEngineVersion();
AGORA_API void *getNativeHandle();
AGORA_API int joinChannel(const char *channelKey, const char *channelName, const char *info, unsigned int uid);
AGORA_API int renewToken(const char *token);
AGORA_API int leaveChannel();
AGORA_API int enableLastmileTest();
AGORA_API int disableLastmileTest();
AGORA_API int enableVideo();
AGORA_API int disableVideo();
AGORA_API int enableLocalVideo(bool enabled);
AGORA_API int startPreview();
AGORA_API int stopPreview();
AGORA_API int enableAudio();
AGORA_API int enableLocalAudio(bool enabled);
AGORA_API int disableAudio();
AGORA_API int setParameters(const char *options);
AGORA_API char *getParameter(const char *parameter, const char *args);
AGORA_API char *getCallId();
AGORA_API int rate(const char *callId, int rating, const char *desc);
AGORA_API int complain(const char *callId, const char *desc);
AGORA_API int setEnableSpeakerphone(int enabled);
AGORA_API int isSpeakerphoneEnabled();
AGORA_API int setDefaultAudioRoutetoSpeakerphone(bool defaultToSpeaker);
AGORA_API int setSpeakerphoneVolume(int volume);
AGORA_API int enableAudioVolumeIndication(int interval, int smooth);
AGORA_API int startAudioRecording(const char *filePath, int quality);
AGORA_API int stopAudioRecording();
AGORA_API int startAudioMixing(const char *filePath, bool loopBack, bool replace, int cycle);
AGORA_API int stopAudioMixing();
AGORA_API int pauseAudioMixing();
AGORA_API int resumeAudioMixing();
AGORA_API int adjustAudioMixingVolume(int volume);
AGORA_API int getAudioMixingDuration();
AGORA_API int getAudioMixingCurrentPosition();
AGORA_API int setAudioMixingPosition(int pos);
AGORA_API int startEchoTest();
AGORA_API int stopEchoTest();
AGORA_API int muteLocalAudioStream(bool muted);
AGORA_API int muteAllRemoteAudioStreams(bool muted);
AGORA_API int muteRemoteAudioStream(unsigned int uid, bool muted);
AGORA_API int switchCamera();
AGORA_API int setVideoProfile(int profile, bool swapWidthAndHeight);
AGORA_API int muteLocalVideoStream(bool muted);
AGORA_API int muteAllRemoteVideoStreams(bool muted);
AGORA_API int muteRemoteVideoStream(unsigned int uid, bool muted);
AGORA_API int setLogFile(const char *filePath);
AGORA_API int setLogFilter(const char *filter);
AGORA_API int setChannelProfile(int profile);
AGORA_API int setClientRole(int role);
AGORA_API int enableDualStreamMode(bool enabled);
AGORA_API int setEncryptionMode(const char *encryptionMode);
AGORA_API int setEncryptionSecret(const char *secret);
AGORA_API int setRemoteVideoStreamType(unsigned int uid, int streamType);
AGORA_API int startRecordingService(const char *recordingKey);
AGORA_API int stopRecordingService(const char *recordingKey);
AGORA_API int refreshRecordingServiceStatus();
AGORA_API int createDataStream(bool reliable, bool ordered);
AGORA_API int sendStreamMessage(int streamId, const char *data, size_t length);
AGORA_API int setRecordingAudioFrameParametersWithSampleRate(int sampleRate, int channel, int mode, int samplesPerCall);
AGORA_API int setPlaybackAudioFrameParametersWithSampleRate(int sampleRate, int channel, int mode, int samplesPerCall);
AGORA_API int setMixedAudioFrameParameters(int sampleRate, int samplesPerCall);
AGORA_API int adjustRecordingSignalVolume(int volume);
AGORA_API int adjustPlaybackSignalVolume(int volume);
AGORA_API int setHighQualityAudioParametersWithFullband(int fullband, int stereo, int fullBitrate);
AGORA_API int enableInEarMonitoring(bool enabled);
AGORA_API int enableWebSdkInteroperability(bool enabled);
AGORA_API int setVideoQualityParameters(bool preferFrameRateOverImageQuality);
AGORA_API int setLocalVoicePitch(double pitch);
AGORA_API int setRemoteVoicePosition(uid_t uid, double pan, double gain);
AGORA_API int setVoiceOnlyMode(bool enabled);
AGORA_API int setPlaybackDeviceVolume(int volume);
AGORA_API int getEffectsVolume();
AGORA_API int setEffectsVolume(int volume);
AGORA_API int playEffect(int soundId, const char *filePath, int loopCount, double pitch, double pan, int gain, bool publish = false);
AGORA_API int stopEffect(int soundId);
AGORA_API int stopAllEffects();
AGORA_API int preloadEffect(int soundId, const char *filePath);
AGORA_API int unloadEffect(int soundId);
AGORA_API int pauseEffect(int soundId);
AGORA_API int pauseAllEffects();
AGORA_API int resumeEffect(int soundId);
AGORA_API int resumeAllEffects();
AGORA_API void freeObject(void *obj);

//video api
AGORA_API int enableVideoObserver();
AGORA_API int disableVideoObserver();
AGORA_API int generateNativeTexture();
AGORA_API int updateTexture(int texId, unsigned char *data, unsigned int uid);
AGORA_API void deleteTexture(int tex);

//----------------Agora Api End-----------------------------
//--------------Callback start------------------
AGORA_API void initCallBackObject();
AGORA_API void initEventOnJoinChannelSuccess(FUNC_OnJoinChannelSuccess onJoinChannelSuccess);
AGORA_API void initEventOnReJoinChannelSuccess(FUNC_OnReJoinChannelSuccess onReJoinChannelSuccess);
AGORA_API void initEventOnConnectionLost(FUNC_OnConnectionLost onConnectionLost);
AGORA_API void initEventOnLeaveChannel(FUNC_OnLeaveChannel onLeaveChannel);
AGORA_API void initEventOnConnectionInterrupted(FUNC_OnConnectionInterrupted onConnectionInterrupted);
AGORA_API void initEventOnRequestToken(FUNC_OnRequestToken onRequestToken);
AGORA_API void initEventOnUserJoined(FUNC_OnUserJoined onUserJoined);
AGORA_API void initEventOnUserOffline(FUNC_OnUserOffline onUserOffline);
AGORA_API void initEventOnAudioVolumeIndication(FUNC_OnAudioVolumeIndication onAudioVolumeIndication);
AGORA_API void initEventOnUserMuteAudio(FUNC_OnUserMuteAudio onUserMuteAudio);
AGORA_API void initEventOnSDKWarning(FUNC_OnSDKWarning onSDKWarning);
AGORA_API void initEventOnSDKError(FUNC_OnSDKError onSDKError);
AGORA_API void initEventOnRtcStats(FUNC_OnRtcStats onRtcStats);
AGORA_API void initEventOnAudioMixingFinished(FUNC_OnAudioMixingFinished onAudioMixingFinished);
AGORA_API void initEventOnAudioRouteChanged(FUNC_OnAudioRouteChanged onAudioRouteChanged);
AGORA_API void initEventOnFirstRemoteVideoDecoded(FUNC_OnFirstRemoteVideoDecoded onFirstRemoteVideoDecoded);
AGORA_API void initEventOnVideoSizeChanged(FUNC_OnVideoSizeChanged onVideoSizeChanged);
AGORA_API void initEventOnClientRoleChanged(FUNC_OnClientRoleChanged onClientRoleChanged);
AGORA_API void initEventOnUserMuteVideo(FUNC_OnUserMuteVideo onUserMuteVideo);
AGORA_API void initEventOnMicrophoneEnabled(FUNC_OnMicrophoneEnabled onMicrophoneEnabled);

AGORA_API bool initEventOnPlaybackAudioFrameBeforeMixing(FUNC_OnPlaybackAudioFrameBeforeMixing onPlaybackAudioFrameBeforeMixing);
//--------------Callback end------------------
#endif