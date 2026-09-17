// #if !UNITY_EDITOR && UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class IOSNativePlugin
{

     [DllImport("__Internal", EntryPoint = "recognizeText")]
     static extern void recognizeText(IntPtr mtlTexture);

     [DllImport("__Internal", EntryPoint = "speak")]
     static extern void speak(string message);

     [DllImport("__Internal", EntryPoint = "stopSpeaking")]
     static extern void stopSpeaking();

     // [DllImport("__Internal", EntryPoint = "getCPUUsage")]
     // static extern float getCPUUsage();

     [DllImport("__Internal", EntryPoint = "getMemoryUsed")]
     static extern float getMemoryUsed();

     [DllImport("__Internal", EntryPoint = "startRecording")]
     static extern float startRecording();

     [DllImport("__Internal", EntryPoint = "stopRecording")]
     static extern float stopRecording();

     [DllImport("__Internal", EntryPoint = "detectFingers")]
     static extern void detectFingers(IntPtr mtlTexture);

     [DllImport("__Internal", EntryPoint = "detectQRCode")]
     static extern void detectQRCode(IntPtr mtlTexture);

     [DllImport("__Internal", EntryPoint = "generateThumbnail")]
     static extern void generateThumbnail(string videoPath, double timeInSeconds);

     [DllImport("__Internal", EntryPoint = "setupAudioSession")]
     private static extern void setupAudioSession();

     [DllImport("__Internal", EntryPoint = "resetAudioSession")]
     private static extern void resetAudioSession();

     [DllImport("__Internal", EntryPoint = "od_load_model")]
     private static extern int od_load_model(string baseNameWithoutExt);

     [DllImport("__Internal", EntryPoint = "od_set_unity_target")]
     private static extern void od_set_unity_target(string goName, string fnName);

     [DllImport("__Internal", EntryPoint = "od_set_thresholds")]
     private static extern void od_set_thresholds(double conf, double iou);

     [DllImport("__Internal", EntryPoint = "od_set_compute_units")]
     private static extern void od_set_compute_units(int mode);
     // 0: All, 1: CPUOnly, 2: CPU+GPU, 3: CPU+NeuralEngine

     [DllImport("__Internal", EntryPoint = "od_prewarm")]
     private static extern double od_prewarm();

     [DllImport("__Internal", EntryPoint = "od_predict_texture")]
     private static extern void od_predict_texture(IntPtr mtlTexture);

#if UNITY_IOS && !UNITY_EDITOR
     [DllImport("__Internal", EntryPoint = "mobileConnectCreateKeyAgreement")]
     static extern IntPtr mobileConnectCreateKeyAgreement(
          string serverPublicKey,
          string sessionId,
          string encryptionKeyId);

     [DllImport("__Internal", EntryPoint = "mobileConnectDecryptEnvelope")]
     static extern IntPtr mobileConnectDecryptEnvelope(
          string nonce,
          string cipherText,
          string authenticationTag,
          string sessionKey,
          string aad);

     [DllImport("__Internal", EntryPoint = "mobileConnectFreeString")]
     static extern void mobileConnectFreeString(IntPtr pointer);
#endif

     public string CreateMobileConnectKeyAgreement(
          string serverPublicKey,
          string sessionId,
          string encryptionKeyId)
     {
#if UNITY_IOS && !UNITY_EDITOR
          var responsePointer = mobileConnectCreateKeyAgreement(
               serverPublicKey,
               sessionId,
               encryptionKeyId);
          if (responsePointer == IntPtr.Zero)
          {
               return null;
          }

          try
          {
               return PtrToStringUtf8(responsePointer);
          }
          finally
          {
               mobileConnectFreeString(responsePointer);
          }
#else
          return null;
#endif
     }

     public string DecryptMobileConnectEnvelope(
          string nonce,
          string cipherText,
          string authenticationTag,
          string sessionKey,
          string aad)
     {
#if UNITY_IOS && !UNITY_EDITOR
          var responsePointer = mobileConnectDecryptEnvelope(
               nonce,
               cipherText,
               authenticationTag,
               sessionKey,
               aad);
          if (responsePointer == IntPtr.Zero)
          {
               return null;
          }

          try
          {
               return PtrToStringUtf8(responsePointer);
          }
          finally
          {
               mobileConnectFreeString(responsePointer);
          }
#else
          return null;
#endif
     }

     public int LoadObjectDetectionModel(string modelName)
     {
          return od_load_model(modelName);
     }

     public void InitializeObjectDetection()
     {
          od_prewarm();
     }

     public void DetectObjects(Texture texture)
     {
          od_predict_texture(texture.GetNativeTexturePtr());
     }

     public void SetupAudioSession()
     {
          setupAudioSession();
     }

     public void ResetAudioSession()
     {
          resetAudioSession();
     }

     public void GenerateThumbnail(string videoPath, double timeInSeconds)
     {
          generateThumbnail(videoPath, timeInSeconds);
     }

     public void DetectQRCode(Texture texture)
     {
          detectQRCode(texture.GetNativeTexturePtr());
     }

     public void StartRecording()
     {
          startRecording();
     }

     public void StopRecording()
     {
          stopRecording();
     }

     public void RecognizeText2(Texture texture)
     {
          recognizeText(texture.GetNativeTexturePtr());
     }

     public void Speak(string message)
     {
          speak(message);
     }

     public void StopSpeaking()
     {
          stopSpeaking();
     }

     public void DetectFingers(Texture texture)
     {
          detectFingers(texture.GetNativeTexturePtr());
     }

     // public float GetCPUUsage()
     // {
     //      return getCPUUsage();
     // }

     public float GetMemoryUsed()
     {
          return getMemoryUsed();
     }

     public void Dispose()
     {
     }

     static string PtrToStringUtf8(IntPtr pointer)
     {
          if (pointer == IntPtr.Zero)
          {
               return null;
          }

          var length = 0;
          while (Marshal.ReadByte(pointer, length) != 0)
          {
               length++;
          }

          var buffer = new byte[length];
          Marshal.Copy(pointer, buffer, 0, length);
          return Encoding.UTF8.GetString(buffer);
     }
}
// #endif
