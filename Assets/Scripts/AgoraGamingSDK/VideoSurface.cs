using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

/* This example script demonstrates how to attach
 * video content to a 3D object (chenzhenyong@agora.io)
 * 
 * Agora engine outputs one local preview video and some
 * remote user video. User ID (int) is used to identify
 * these video streams. 0 is used for local preview video
 * stream, and other value stands for remote user video
 * stream.
 */

public class VideoSurface : MonoBehaviour
{

    private System.IntPtr data = Marshal.AllocHGlobal(1920 * 1080 * 4);
    private int defWidth = 0;
    private int defHeight = 0;
    private Texture2D nativeTexture;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // process engine messages (TODO: put in some other place)
        agora_gaming_rtc.IRtcEngine engine = agora_gaming_rtc.IRtcEngine.QueryEngine();
        if (engine == null)
            return;

        // render video
        Renderer rend = GetComponent<Renderer>();
        uint uid = mUid;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
        if (mEnable)
        {
            // create texture if not existent
            if (rend.material.mainTexture == null)
            { 
                int tmpi = engine.UpdateTexture(0, uid, data, ref defWidth, ref defHeight);

                if (tmpi == -1) {
                    return;
                }

                if (defWidth > 0 && defHeight > 0)
                {
                    try
                    {
                        // create Texture in the first time update data
                        nativeTexture = new Texture2D((int)defWidth, (int)defHeight, TextureFormat.RGBA32, false);
                        rend.material.mainTexture = nativeTexture;
                        nativeTexture.LoadRawTextureData(data, (int)defWidth * (int)defHeight * 4);
                        nativeTexture.Apply();
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Exception e = " + e);
                    }
                }
            }
            else if (rend.material.mainTexture != null && rend.material.mainTexture is Texture2D)
            {       
                    int width = 0;
                    int height = 0;
                    int tmpi = engine.UpdateTexture(0, uid, data, ref width, ref height);
                    if (tmpi == -1) {
                        return;
                    }

                    if (width == defWidth  && height == defHeight)
                    {
                        try
                        {
                            /*
                            *  if width and height don't change ,we only need to update data for texture, do not need to create Texture.
                            */
                            nativeTexture.LoadRawTextureData(data, (int)width * (int)height * 4);
                            nativeTexture.Apply();
                        }
                        catch (System.Exception e)
                        {
                            Debug.Log("Exception e = " + e);
                        }
                        
                    } else {
                        try
                        {
                            /* 
                            * if width or height changed ,we need to create new texture.
                            */
                            defWidth = width;
                            defHeight = height;
                            nativeTexture = null;
                            nativeTexture = new Texture2D ((int)defWidth, (int)defHeight, TextureFormat.RGBA32, false);
                            rend.material.mainTexture = nativeTexture;
                         }
                        catch (System.Exception e)
                        {
                            Debug.Log("Exception e = " + e);
                        }
                    }
            }
        }
        else
        {
            if (rend.material.mainTexture != null && rend.material.mainTexture is Texture2D)
            {
                rend.material.mainTexture = null;
            }
        }

#else
       if (mEnable) {
			// create texture if not existent
			if (rend.material.mainTexture == null) {
                System.IntPtr texPtr = (IntPtr)engine.GenerateNativeTexture();
                Texture2D nativeTexture = Texture2D.CreateExternalTexture(640, 360, TextureFormat.ARGB32, false, false, texPtr); // FIXME! texture size is subject to change
                rend.material.mainTexture = nativeTexture;
            }
            if (rend.material.mainTexture != null && rend.material.mainTexture is Texture2D) {
                Texture2D tex = rend.material.mainTexture as Texture2D;
                int texId = (int)tex.GetNativeTexturePtr();
                int texWidth = 0;
				int texHeight = 0;
				if(engine.UpdateTexture (texId, uid, data, ref texWidth, ref texHeight) == 0) {
                    // TODO: process texture then render
                }
			}
		
    } else {
			if (rend.material.mainTexture != null && rend.material.mainTexture is Texture2D) {
				Texture2D tex = rend.material.mainTexture as Texture2D;
				int texId = (int)tex.GetNativeTexturePtr ();
				engine.DeleteTexture(texId);
				rend.material.mainTexture = null;
			}
		}
#endif
    }

    void OnDestroy()
    {
        Marshal.FreeHGlobal(data);
        Debug.Log("OnDestroy");
    }

    // call this to render video stream from uid on this game object
    public void SetForUser(uint uid)
    {
        mUid = uid;
        Debug.Log("Set uid " + uid + " for " + gameObject.name);
    }

    /*
     * if enable = true, the video will render. if enable = false, the video will stop render.
     */
    public void SetEnable(bool enable)
    {
        mEnable = enable;
    }

    public delegate void SetTransformDelegate(uint uid, string objName, ref Transform transform);

    public SetTransformDelegate mAdjustTransfrom = null;

    /*
     * uid = 0, it means yourself but not others, you can get others uid by Agora Engine CallBack onUserJoined.
     */
    private uint mUid = 0;

    /*
     *if disabled, then no rendering happens
     */
    private bool mEnable = true;
}
