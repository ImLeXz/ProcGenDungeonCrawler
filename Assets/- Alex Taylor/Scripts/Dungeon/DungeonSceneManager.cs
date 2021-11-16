using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ATDungeon.SceneManagement
{
    public class DungeonSceneManager : MonoBehaviour
    {
        [SerializeField]
        private float invokeDelay;
        [SerializeField]
        private Animator transitionAnimator;
        [SerializeField]
        private int lightingSceneBuildIndex;
        [SerializeField]
        private int mainMenuSceneBuildIndex;
        [SerializeField]
        private int settingsMenuSceneBuildIndex;
        [SerializeField]
        private int statisticsSceneBuildIndex;
        [SerializeField]
        private int classSelectionSceneBuildIndex;
        [SerializeField]
        private int mainSceneBuildIndex;

        public static DungeonSceneManager Instance;
        float transitionWaitTime = 0.0f;

        private void Awake()
        {
            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(LoadScene(lightingSceneBuildIndex, "", LoadSceneMode.Additive));
            StartCoroutine(LoadScene(mainMenuSceneBuildIndex, "", LoadSceneMode.Additive));
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        // Update is called once per frame
        void Update()
        {
            //DebugControls();
        }

        private void DebugControls()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                ReloadScene(mainSceneBuildIndex);
            }
        }

        public void ReloadDungeon()
        {
            ReloadScene(mainSceneBuildIndex);
        }

        public void ReloadScene(int sceneIndex)
        {
            UnloadScene(sceneIndex);
            StartCoroutine(LoadScene(sceneIndex, "crossfade", LoadSceneMode.Additive));
        }

        public IEnumerator LoadScene(int sceneIndex, string transitionName, LoadSceneMode loadSceneMode, Scene sceneToUnload)
        {
            if (transitionName != "")
            {
                PlayTransition(transitionName);
                Invoke("GetTransitionWaitTime", invokeDelay);
                yield return new WaitForSeconds(transitionWaitTime + invokeDelay);
            }

            if (sceneToUnload != null)
                UnloadScene(sceneToUnload);

            SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);
        }

        public IEnumerator LoadScene(int sceneIndex, string transitionName, LoadSceneMode loadSceneMode)
        {
            if (transitionName != "")
            {
                PlayTransition(transitionName);
                Invoke("GetTransitionWaitTime", invokeDelay);
                yield return new WaitForSeconds(transitionWaitTime + invokeDelay);
            }

            SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);
        }

        private void GetTransitionWaitTime()
        {
            AnimatorStateInfo asi = transitionAnimator.GetCurrentAnimatorStateInfo(1);
            transitionWaitTime = asi.length;
            Debug.Log("Transition Wait Time Set To: " + transitionWaitTime);
        }

        public void UnloadScene(int sceneIndex)
        {
            SceneManager.UnloadSceneAsync(sceneIndex);
        }

        public void UnloadScene(Scene scene)
        {
            SceneManager.UnloadSceneAsync(scene);
        }

        public void PlayTransition(string name)
        {
            if (name != "")
                transitionAnimator.SetBool(name, true);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name);

            if (scene.buildIndex == lightingSceneBuildIndex)
            {
                Debug.Log("Switching Active Scene");
                SceneManager.SetActiveScene(scene);
            }

            else if(scene.buildIndex == mainSceneBuildIndex)
            {
                Debug.Log("Loading Main Game, Setting Up References In PersistenceManager");

                //Reference Setup
                Persistence.PersistenceManager.Instance.SetupPlayerView();
            }
        }

        public int GetGameSceneBuildIndex() { return mainSceneBuildIndex; }
        public int GetMainMenuSceneBuildIndex() { return mainMenuSceneBuildIndex; }
        public int GetClassSelectionSceneBuildIndex() { return classSelectionSceneBuildIndex; }
        public int GetSettingsMenuSceneBuildIndex() { return settingsMenuSceneBuildIndex; }
        public int GetStatisticsSceneBuildIndex() { return statisticsSceneBuildIndex; }
    }
}
