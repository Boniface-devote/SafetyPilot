using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class VoiceManager : MonoBehaviour
{
    public Button voiceButton;
    public TMP_Text statusText;
    public TMP_InputField userInputField;

    private string apiUrl = "https://api.groq.com/openai/v1/audio/transcriptions";
    private string apiKey = "Bearer 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";
    private AudioClip recordedClip;
    private bool isRecording = false;
    private string filePath;

    void Start()
    {
        voiceButton.onClick.AddListener(ToggleRecording);
        statusText.text = "";
        filePath = Application.persistentDataPath + "/recordedAudio.wav";
    }

    void ToggleRecording()
    {
        if (isRecording)
        {
            StopRecording();
        }
        else
        {
            StartRecording();
        }
    }

    void StartRecording()
    {
        statusText.text = "Listening...";
        recordedClip = Microphone.Start(null, false, 10, 44100);
        isRecording = true;
    }

    void StopRecording()
    {
        if (!isRecording) return;

        Microphone.End(null);
        isRecording = false;
        statusText.text = "Processing...";
        SaveWavFile();
        StartCoroutine(SendAudioToGroq());
    }

    void SaveWavFile()
    {
        var samples = new float[recordedClip.samples * recordedClip.channels];
        recordedClip.GetData(samples, 0);
        byte[] wavData = ConvertToWav(recordedClip);
        File.WriteAllBytes(filePath, wavData);
    }

    byte[] ConvertToWav(AudioClip clip)
    {
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            int sampleCount = clip.samples * clip.channels;
            int sampleRate = clip.frequency;
            writer.Write(new char[] { 'R', 'I', 'F', 'F' });
            writer.Write(36 + sampleCount * 2);
            writer.Write(new char[] { 'W', 'A', 'V', 'E' });
            writer.Write(new char[] { 'f', 'm', 't', ' ' });
            writer.Write(16);
            writer.Write((short)1);
            writer.Write((short)clip.channels);
            writer.Write(sampleRate);
            writer.Write(sampleRate * clip.channels * 2);
            writer.Write((short)(clip.channels * 2));
            writer.Write((short)16);
            writer.Write(new char[] { 'd', 'a', 't', 'a' });
            writer.Write(sampleCount * 2);

            float[] samples = new float[sampleCount];
            clip.GetData(samples, 0);
            foreach (var sample in samples)
            {
                writer.Write((short)(sample * short.MaxValue));
            }
            return stream.ToArray();
        }
    }

    IEnumerator SendAudioToGroq()
    {
        byte[] audioData = File.ReadAllBytes(filePath);
        WWWForm form = new WWWForm();
        form.AddField("model", "whisper-large-v3");
        form.AddBinaryData("file", audioData, "audio.wav", "audio/wav");
        form.AddField("response_format", "verbose_json");

        using (UnityWebRequest request = UnityWebRequest.Post(apiUrl, form))
        {
            request.SetRequestHeader("Authorization", apiKey);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                string transcribedText = ParseTranscription(jsonResponse);
                userInputField.text = transcribedText;
                statusText.text = "";
            }
            else
            {
                statusText.text = "Error: Could not transcribe.";
                Debug.LogError(request.error);
            }
        }
    }

    string ParseTranscription(string jsonResponse)
    {
        // Basic JSON parsing; adapt as needed based on actual API response
        int startIdx = jsonResponse.IndexOf("\"text\":\"") + 8;
        int endIdx = jsonResponse.IndexOf("\"", startIdx);
        return jsonResponse.Substring(startIdx, endIdx - startIdx);
    }
}
