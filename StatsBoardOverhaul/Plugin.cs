using System;
using System.Reflection;
using BepInEx;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace StatsBoardOverhaul
{
    [BepInPlugin(Constants.GUID, Constants.Name, Constants.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private static GameObject _statsBoardobj;
        private static Text _playerHeader, _timePlayedS, _timePlayedT, _currentTime, _currentDate, _playerColour, _tagsFromP, _tagsFromO;
        private static Image _furDisplay;
        private float _timeS, _timeT;

        void Start() => GorillaTagger.OnPlayerSpawned(Initialization);

        void Initialization()
        {
            try
            {
                if (_statsBoardobj == null)
                {
                    _statsBoardobj = Instantiate(InitialiseBoard("StatsBoardOverhaul.Resource.statsboard").LoadAsset<GameObject>("StatsBoard"));
                    _statsBoardobj.transform.SetLocalPositionAndRotation(new Vector3(-62.6963f, 12.4685f, -83.714f), Quaternion.Euler(10.1947f, 276.0343f, 0f));
                    _statsBoardobj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    Logger.LogInfo($"SUCCESSFULLY!, Initialized {Constants.Name}");

                    _playerHeader = _statsBoardobj.transform.Find("UI/Canvas/Stats/Username").gameObject.GetComponent<Text>();
                    _timePlayedT = _statsBoardobj.transform.Find("UI/Canvas/Stats/TimeplayedT").gameObject.GetComponent<Text>();
                    _timePlayedS = _statsBoardobj.transform.Find("UI/Canvas/Stats/TimePlayedS").gameObject.GetComponent<Text>();
                    _currentTime = _statsBoardobj.transform.Find("UI/Canvas/Stats/Time").gameObject.GetComponent<Text>();
                    _currentDate = _statsBoardobj.transform.Find("UI/Canvas/Stats/Date").gameObject.GetComponent<Text>();
                    _playerColour = _statsBoardobj.transform.Find("UI/Canvas/Stats/Usercolour").gameObject.GetComponent<Text>();

                    // _tagsFromP = _statsBoardobj.transform.Find("UI/Canvas/Stats/").gameObject.GetComponent<Text>();
                    // _tagsFromO = _statsBoardobj.transform.Find("UI/Canvas/Stats/").gameObject.GetComponent<Text>();

                    _furDisplay = _statsBoardobj.transform.Find("UI/Canvas/Stats/Userswatch").GetComponent<Image>();

                    ApplyContent();
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"ERROR!, With Initializing {Constants.Name} {e}");
            }
        }

        void ApplyContent()
        {
            if (PlayerPrefs.HasKey(Constants.KeyType))
            {
                _timeT = PlayerPrefs.GetFloat(Constants.KeyType);
            }
        }

        void Update()
        {
            _timeS += Time.deltaTime;
            _timeT += Time.deltaTime;

            TimeSpan _timeSpanS = TimeSpan.FromSeconds(_timeS);
            TimeSpan _timeSpanT = TimeSpan.FromSeconds(_timeT);

            _timePlayedT.text = $"Time Played ( Total ): {_timeSpanT.Hours}H, {_timeSpanT.Minutes}M, {_timeSpanT.Seconds}S";
            _timePlayedS.text = $"Time Played ( Session ): {_timeSpanS.Hours}H, {_timeSpanS.Minutes}M, {_timeSpanS.Seconds}S";

            var colour = GorillaTagger.Instance.offlineVRRig.playerColor;
            _playerColour.text = $"Colour: R: <color=yellow>{Mathf.RoundToInt(colour.r * 9)}</color>, G: <color=yellow>{Mathf.RoundToInt(colour.g * 9)}</color>, B: <color=yellow>{Mathf.RoundToInt(colour.b * 9)}</color>";

            _playerHeader.text = $"Name: {PhotonNetwork.LocalPlayer.NickName}";

            DateTime now = DateTime.Now;
            _currentTime.text = now.ToString("hh:mm tt").ToUpper();
            _currentDate.text = now.ToString("dddd dd MMMM yyyy").ToUpper();

            if (GorillaTagger.Instance.offlineVRRig != null)
            {
                _furDisplay.color = GorillaTagger.Instance.offlineVRRig.playerColor;
            }
        }

        public AssetBundle InitialiseBoard(string path)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            return AssetBundle.LoadFromStream(stream);
        }

        void OnApplicationQuit()
        {
            PlayerPrefs.SetFloat(Constants.KeyType, Mathf.Round(_timeT));
            PlayerPrefs.Save();
        }
    }
}
