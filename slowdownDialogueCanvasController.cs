using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gamekit3D
{
    public class slowdownDialogueCanvasController : MonoBehaviour
    {

        public Animator animator;
        public TextMeshProUGUI textMeshProUGUI;
        //public float delaytime = 0f;
        //public float delaytime1 = 5f;
        protected Coroutine m_DeactivationCoroutine;
        public GameObject pausetext;
        public GameObject coroutinemanager;

        protected readonly int m_HashActivePara = Animator.StringToHash("Active");

        IEnumerator SetAnimatorParameterWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.SetBool(m_HashActivePara, false);
        }

        public void ActivateCanvasWithText(string text)
        {
            if (m_DeactivationCoroutine != null)
            {
                StopCoroutine(m_DeactivationCoroutine);
                m_DeactivationCoroutine = null;
            }

            gameObject.SetActive(true);
            animator.SetBool(m_HashActivePara, true);
            textMeshProUGUI.text = text;

        }
        public void destroycoroutine()
        {
            Destroy(coroutinemanager);
        }

        public void slowdown()
        {
            coroutinemanager.SetActive(true);
        }

        public void ActivateCanvasWithTranslatedText(string phraseKey)
        {
            if (m_DeactivationCoroutine != null)
            {
                StopCoroutine(m_DeactivationCoroutine);
                m_DeactivationCoroutine = null;
            }

            gameObject.SetActive(true);
            animator.SetBool(m_HashActivePara, true);
            textMeshProUGUI.text = Translator.Instance[phraseKey];
        }

        public void DeactivateCanvasWithDelay(float delay)
        {
            m_DeactivationCoroutine = StartCoroutine(SetAnimatorParameterWithDelay(delay));
        }
    }
}
