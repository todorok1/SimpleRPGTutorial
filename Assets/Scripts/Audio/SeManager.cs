using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleRpg
{
    /// <summary>
    /// 効果音を管理するクラスです。
    /// </summary>
    public class SeManager : MonoBehaviour
    {
        /// <summary>
        /// 生成済みのAudioSourceを保持するリストです。
        /// </summary>
        List<AudioSource> _audioSourceList = new();

        /// <summary>
        /// 効果音の音量です。
        /// </summary>
        readonly float Volume = 0.75f;

        /// <summary>
        /// チャンネル用ゲームオブジェクトの名前の接頭辞です。
        /// </summary>
        readonly string ChannelPrefix = "SE Channel";

        /// <summary>
        /// 効果音を再生します。
        /// </summary>
        /// <param name="seName">再生する効果音の名前</param>
        /// <param name="isLoop">ループ再生するかどうか</param>
        public void Play(string seName, bool isLoop = false)
        {
            // 効果音のアドレスを取得します。
            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(seName);
            handle.Completed += (operation) =>
            {
                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    AudioSource audioSource = GetAudioSource();
                    audioSource.clip = operation.Result;
                    audioSource.loop = isLoop;
                    audioSource.volume = Volume;
                    audioSource.Play();
                }
                else
                {
                    SimpleLogger.Instance.LogError($"指定された効果音が見つかりません: {seName}");
                }
            };
        }

        /// <summary>
        /// オーディオを再生するAudioSourceを取得します。
        /// </summary>
        public AudioSource GetAudioSource()
        {
            var audioSource = GetStoppedAudioSource();
            if (audioSource != null)
            {
                return audioSource;
            }

            // すべてのAudioSourceが再生中の場合、新しいAudioSourceを生成します。
            GameObject audioSourceObject = new(ChannelPrefix);
            audioSourceObject.transform.SetParent(transform);
            AudioSource newAudioSource = audioSourceObject.AddComponent<AudioSource>();
            _audioSourceList.Add(newAudioSource);
            return newAudioSource;
        }

        /// <summary>
        /// 停止しているAudioSourceを取得します。
        /// </summary>
        public AudioSource GetStoppedAudioSource()
        {
            foreach (var audioSource in _audioSourceList)
            {
                if (!audioSource.isPlaying)
                {
                    return audioSource;
                }
            }
            return null;
        }
    }
}