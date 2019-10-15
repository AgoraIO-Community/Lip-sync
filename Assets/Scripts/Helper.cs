using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class Helper
{
    public static GameObject Spawn(GameObject prefab, Transform parent = null)
    {
        GameObject newObject = Object.Instantiate(prefab);
        newObject.transform.SetParent(parent);
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localRotation = Quaternion.identity;
        newObject.transform.localScale = prefab.transform.localScale;
        newObject.SetActive(true);
        return newObject;
    }

    public static Transform[] GetAllChildren(Transform parent)
    {
        Transform[] result = new Transform[parent.childCount];

        for (int i = 0; i < parent.childCount; i++)
        {
            result[i] = parent.GetChild(i);
        }

        return result;
    }

    public static void MoveAllChildren(Transform oldParent, Transform newParent)
    {
        for (int i = oldParent.childCount - 1; i >= 0; i--)
        {
            oldParent.GetChild(i).SetParent(newParent);
        }
    }

    public static void RemoveAllChildren(Transform parent, bool isInstant = false)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            if (isInstant)
            {
                Object.DestroyImmediate(parent.GetChild(i).gameObject);
            }
            else
            {
                parent.GetChild(i).gameObject.SetActive(false);
                Object.Destroy(parent.GetChild(i).gameObject);
            }
        }
    }

    public static Vector2 ConvertWorldPositionToCanvasPosition(Canvas canvas, Vector3 worldPosition,
        Camera camera = null)
    {
        if (camera == null)
            camera = Camera.main;

        Vector2 screenPosition = camera.WorldToScreenPoint(worldPosition);
        return ConvertScreenPositionToCanvasPosition(canvas, screenPosition);
    }

    public static Vector2 ConvertScreenPositionToCanvasPosition(Canvas canvas, Vector2 screenPosition)
    {
        Vector2 result = screenPosition;
        result.x -= Screen.width / 2f;
        result.y -= Screen.height / 2f;
        result /= canvas.scaleFactor;
        return result;
    }

    public static Vector3 GetRealCanvasElementPosition(RectTransform canvasRectTransform, Vector3 elementPosition)
    {
        return canvasRectTransform.InverseTransformPoint(elementPosition);
    }

    public static int RandomEnum<T>() where T : struct, IComparable, IConvertible, IFormattable
    {
        if (typeof(T).IsEnum)
            return Random.Range(0, Enum.GetNames(typeof(T)).Length);
        return 0;
    }

    public static Color ConvertColorToGrayscale(Color color, bool fixTransparent = false)
    {
        if (fixTransparent && color == Color.clear)
            color = Color.white;

        //double luminance = 0.2126 * color.r + 0.7152 * color.g + 0.0722 * color.b;
        //float luminanceRounded = (float)luminance;
        //Color result = new Color(luminanceRounded, luminanceRounded, luminanceRounded);
        float grayscale = color.grayscale;
        Color result = new Color(grayscale, grayscale, grayscale);
        return result;
    }

    public static Texture2D ConvertTextureToGrayscale(Texture2D source, bool fixTransparent = false)
    {
        Texture2D result = new Texture2D(source.width, source.height, source.format, false);
        result.filterMode = source.filterMode;

        Color[] colorArray = source.GetPixels();

        for (int i = 0; i < colorArray.Length; i++)
        {
            colorArray[i] = ConvertColorToGrayscale(colorArray[i], fixTransparent);
        }

        result.SetPixels(colorArray);
        result.Apply();
        return result;
    }

    public static void SortChildByName(Transform parent)
    {
        List<Transform> childList = GetAllChildren(parent).ToList();
        childList.Sort((x1, x2) => string.CompareOrdinal(x1.name, x2.name));

        for (int i = 0; i < childList.Count; i++)
        {
            Transform child = childList[i];
            child.SetSiblingIndex(i);
        }
    }

    public static Texture2D MakeSquare(Texture2D source)
    {
        if (source.width == source.height)
            return source;

        int targetSize;
        int offset;
        bool isPreserveWidth;

        if (source.width < source.height)
        {
            targetSize = source.width;
            offset = (int)((source.height - source.width) / 2f);
            isPreserveWidth = true;
        }
        else
        {
            targetSize = source.height;
            offset = (int)((source.width - source.height) / 2f);
            isPreserveWidth = false;
        }

        Texture2D result = new Texture2D(targetSize, targetSize, source.format, false);
        result.filterMode = source.filterMode;

        Color[] colorArray = new Color[targetSize * targetSize];

        for (int i = 0; i < targetSize; i++)
        {
            for (int j = 0; j < targetSize; j++)
            {
                int index = i + j * targetSize;
                int x = 0;
                int y = 0;

                if (isPreserveWidth)
                {
                    x = i;
                    y = j + offset;
                }
                else
                {
                    x = i + offset;
                    y = j;
                }

                colorArray[index] = source.GetPixel(x, y);
            }
        }

        result.SetPixels(colorArray);
        result.Apply();
        return result;
    }

    public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        result.filterMode = source.filterMode;

        Color[] colorArray = new Color[targetWidth * targetHeight];

        float stepX = source.width / (float)targetWidth;
        float stepY = source.height / (float)targetHeight;

        for (int i = 0; i < targetWidth; i++)
        {
            for (int j = 0; j < targetHeight; j++)
            {
                int x = Mathf.RoundToInt(i * stepX);
                int y = Mathf.RoundToInt(j * stepY);
                colorArray[i + j * targetWidth] = source.GetPixel(x, y);
            }
        }

        result.SetPixels(colorArray);
        result.Apply();
        return result;
    }

    public static string LoadTextFromFile(string path)
    {
        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line = sr.ReadToEnd();
                return line;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public static void SaveToFile(string text, string path, string name)
    {
        string finalPath = Path.Combine(path, name);
        StreamWriter sw = new StreamWriter(finalPath);
        sw.WriteLine(text);
        sw.Close();
    }

    public static string FixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }


    public static Texture2D LoadTextureFromFile(string path)
    {
        string realPath = path;

        if (!File.Exists(realPath))
        {
            realPath = path + ".png";

            if (!File.Exists(realPath))
            {
                realPath = path + ".jpg";

                if (!File.Exists(realPath))
                    return null;
            }
        }

        byte[] fileData = File.ReadAllBytes(realPath);
        Texture2D result = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        result.LoadImage(fileData);
        return result;
    }

    public static void SaveTextureToFile(Texture2D texture, string path)
    {
        byte[] bytes = null;

        string extension = path.Substring(path.Length - 4);

        if (extension == ".png")
            bytes = texture.EncodeToPNG();
        else if (extension == ".jpg")
            bytes = texture.EncodeToJPG();

        File.WriteAllBytes(path, bytes);
    }

    public static Texture2D ScalePixelizedTexture(Texture2D texture, int scaleMax)
    {
        int width = texture.width;
        int height = texture.height;

        Texture2D newTexture = new Texture2D(width * scaleMax, height * scaleMax);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //get one pixel color
                Color color = texture.GetPixel(j, i);
                Color[] colors = new Color[scaleMax * scaleMax];

                for (int n = 0; n < scaleMax * scaleMax; n++)
                {
                    colors[n] = color;
                }

                //set pixel to the bigger texture
                newTexture.SetPixels(j * scaleMax, i * scaleMax, scaleMax, scaleMax, colors);
            }
        }

        return newTexture;
    }

    public static DateTime GetBaseDateTime()
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    public static double GetCurrentUnixTimestamp()
    {
        return DateTime.UtcNow.Subtract(GetBaseDateTime()).TotalSeconds;
    }

    public static DateTime ConvertUnixToDateTime(long totalSeconds)
    {
        return GetBaseDateTime().AddSeconds(totalSeconds);
    }

    public static void ClampImageSize(Graphic image, float max)
    {
        float width = image.mainTexture.width;
        float height = image.mainTexture.height;

        if (width > height)
        {
            float ratio = max / width;
            float newWidth = ratio * width;
            float newHeight = ratio * height;
            image.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        }
        else
        {
            float ratio = max / height;
            float newWidth = ratio * width;
            float newHeight = ratio * height;
            image.rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        }
    }

    public static Bounds CalculateBoundingBox(GameObject gameObject)
    {
        Quaternion currentRotation = gameObject.transform.rotation;
        gameObject.transform.rotation = Quaternion.identity;
        Bounds bounds = new Bounds(gameObject.transform.position, Vector3.zero);

        foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }

        Vector3 localCenter = bounds.center - gameObject.transform.position;
        bounds.center = localCenter;
        gameObject.transform.rotation = currentRotation;
        return bounds;
    }

    public static float CalculateScaleFactorToFit(Bounds bounds, float size)
    {
        float maxBoundSize = Math.Max(bounds.size.x, Math.Max(bounds.size.y, bounds.size.z));
        return size / maxBoundSize;
    }

    public static void DrawLine(ref Texture2D tex, int x0, int y0, int x1, int y1, Color col)
    {
        int dy = (int)(y1 - y0);
        int dx = (int)(x1 - x0);
        int stepx, stepy;

        if (dy < 0) { dy = -dy; stepy = -1; }
        else { stepy = 1; }
        if (dx < 0) { dx = -dx; stepx = -1; }
        else { stepx = 1; }
        dy <<= 1;
        dx <<= 1;

        float fraction = 0;

        tex.SetPixel(x0, y0, col);
        if (dx > dy)
        {
            fraction = dy - (dx >> 1);
            while (Mathf.Abs(x0 - x1) > 1)
            {
                if (fraction >= 0)
                {
                    y0 += stepy;
                    fraction -= dx;
                }
                x0 += stepx;
                fraction += dy;
                tex.SetPixel(x0, y0, col);
            }
        }
        else
        {
            fraction = dx - (dy >> 1);
            while (Mathf.Abs(y0 - y1) > 1)
            {
                if (fraction >= 0)
                {
                    x0 += stepx;
                    fraction -= dy;
                }
                y0 += stepy;
                fraction += dx;
                tex.SetPixel(x0, y0, col);
            }
        }
    }

    public static void CleanUp()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    public static IEnumerator CleanUpCoroutine(float timer)
    {
        yield return new WaitForSeconds(timer);

        CleanUp();
    }

    public enum EScreenAspect
    {
        None,
        TwoOne,
        OneSixNine,
        OneSixOneZero,
        ThreeTwo,
        FourThree,
        TwoOnePortrail,
        OneSixNinePortrail,
        OneSixOneZeroPortrail,
        ThreeTwoPortrail,
        FourThreePortrail
    }

    public static EScreenAspect GetScreenAspect()
    {
        EScreenAspect result = EScreenAspect.None;
        float ratio = (float)Screen.width / (float)Screen.height;

        if (ratio >= 2)// 2:1
        {
            result = EScreenAspect.TwoOne;
        }
        else if (ratio >= 16f / 9f)// 16:9
        {
            result = EScreenAspect.OneSixNine;
        }
        else if (ratio >= 1.6f)// 16:10
        {
            result = EScreenAspect.OneSixOneZero;
        }
        else if (ratio >= 1.5f)// 3:2
        {
            result = EScreenAspect.ThreeTwo;
        }
        else if (ratio >= 4f / 3f)// 4:3
        {
            result = EScreenAspect.FourThree;
        }

        if (result == EScreenAspect.None)
        {
            ratio = (float)Screen.height / (float)Screen.width;

            if (ratio >= 2)// 2:1
            {
                result = EScreenAspect.TwoOnePortrail;
            }
            else if (ratio >= 16f / 9f)// 16:9
            {
                result = EScreenAspect.OneSixNinePortrail;
            }
            else if (ratio >= 1.6f)// 16:10
            {
                result = EScreenAspect.OneSixOneZeroPortrail;
            }
            else if (ratio >= 1.5f)// 3:2
            {
                result = EScreenAspect.ThreeTwoPortrail;
            }
            else if (ratio >= 4f / 3f)// 4:3
            {
                result = EScreenAspect.FourThreePortrail;
            }
        }

        return result;
    }

    public static Color colorCorection(Color color, float m_GammaCorrection)
    {
        Color newColor = Color.clear;

        newColor.r = Mathf.Pow(color.r, 1.0f / m_GammaCorrection);
        newColor.g = Mathf.Pow(color.g, 1.0f / m_GammaCorrection);
        newColor.b = Mathf.Pow(color.b, 1.0f / m_GammaCorrection);
        newColor.a = Mathf.Pow(color.a, 1.0f / m_GammaCorrection);

        return newColor;
    }
	
	public static bool DetectLandscape(){
		if (DetectIpad() && (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight
            ))
            return true;
        return false;
	}
	
	public static bool DetectIpad(){
		#if UNITY_IOS
		if(UnityEngine.iOS.Device.generation.ToString().Contains("iPad")){
			return true;
		}
		#endif
		return false;
	}

    

    public static Texture2D AddPixelsToTextureIfClear(Texture2D texture, Color[] newPixels, int placementX, int placementY, int placementWidth, int placementHeight)
    {
        if (placementX + placementWidth < texture.width)
        {
            texture.SetPixels(placementX, placementY, placementWidth, placementHeight, newPixels);
        }
        else
        {
            UnityEngine.Debug.Log("Letter Falls Outside Bounds of Texture" + (placementX + placementWidth));
        }


        return texture;
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static float ReformatFloat(float origin, int offset) {
        float result = 0;

        int multyValue = 1;
        for (int i = 0; i < offset; i++)
        {
            multyValue *= 10;
        }

        float originInt = Mathf.Round(origin * multyValue);

        result = originInt / multyValue;

        return result;
    }

    public static byte[] ToByteArray(float[] floatArray)
    {
        int len = floatArray.Length * 4;
        byte[] byteArray = new byte[len];
        int pos = 0;
        foreach (float f in floatArray)
        {
            byte[] data = System.BitConverter.GetBytes(f);
            System.Array.Copy(data, 0, byteArray, pos, 4);
            pos += 4;
        }
        return byteArray;
    }

    public static float[] ToFloatArray(byte[] byteArray)
    {
        int len = byteArray.Length / 4;
        float[] floatArray = new float[len];
        for (int i = 0; i < byteArray.Length; i += 4)
        {
            floatArray[i / 4] = System.BitConverter.ToSingle(byteArray, i);
        }
        return floatArray;
    }

    public static Vector3 getRandomCharacterPos() {
        Vector3 position = Vector3.zero;

        float randomX = Random.Range(-3f, 3f);

        float randomZ = Random.Range(-3f, 3f);

        position = new Vector3(randomX, 0, randomZ);

        return position;
    }

    public static float[] ConvertByteToFloat(byte[] array)
    {
        float[] floatArr = new float[array.Length / 4];
        for (int i = 0; i < floatArr.Length; i++)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(array, i * 4, 4);
            //floatArr[i] = BitConverter.ToSingle(array, i * 4);

            floatArr[i] = BitConverter.ToSingle(array, i * 4) / 0x80000000;
        }
        return floatArr;
    }

    public static float[] PCM2Floats(byte[] bytes, bool isLittleEndian = false)
    {
        float max = -(float)System.Int16.MinValue;
        float[] samples = new float[bytes.Length / 2];

        //Debug.Log("sample value max is " + max);

        for (int i = 0; i < samples.Length; i++)
        {
            short int16sample;

            if (isLittleEndian)
                int16sample = GetShortFromLittleEndianBytes(bytes, i * 2);
            else 
                int16sample = System.BitConverter.ToInt16(bytes, i * 2);


            //if (int16sample != 0)
            //    Debug.Log("sample value int16sample is " + int16sample);

            samples[i] = (float)int16sample / max;

            //if (samples[i] != 0)
            //    Debug.Log("sample value samples[i] is " + samples[i]);
        }

        return samples;
    }

    public static short GetShortFromLittleEndianBytes(byte[] data, int startIndex)
    {
        return (short)((data[startIndex + 1] << 8)
             | data[startIndex]);
    }

    public static float[] PCMBytesToFloats(byte[] bytes)
    {
        float[] floats = new float[bytes.Length / 2];
        for (int i = 0; i < bytes.Length; i += 2)
        {
            short s = BitConverter.ToInt16(bytes, i); // convert 2 bytes to short
            floats[i / 2] = ((float)s) / (float)32768; // convert short to float
        }
        return floats;
    }
}