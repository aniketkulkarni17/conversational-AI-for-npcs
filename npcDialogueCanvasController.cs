using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Gamekit3D
{
    public class npcDialogueCanvasController : MonoBehaviour
    {
        public string input;
        public Animator animator;
        public TextMeshProUGUI textMeshProUGUI;

        //socket declarations
        public string connectionIP = "127.0.0.1";
        public int connectionPort = 25001;
        IPAddress localAdd;
        TcpListener listener;
        System.Net.Sockets.TcpClient client;

        public string received_input;
        public bool inInfo = false;

        //soundtracks
        public AudioSource sad_soundtrack;
        public AudioSource happy_soundtrack;
        public AudioSource background_soundtrack;
        public AudioSource postcutscene_soundtrack;

        //cutscene
        public GameObject cutscene;
        public int cutscene_duration;
        public int questanimation_duration;
        public int sa_animation_duration;

        //notification animation game objects
        public GameObject quest_start_notification; //quest notification animations
        public GameObject quest_end_notification;
        public GameObject optional_q_notification;

        public GameObject sa_sad_notification; //soundtrack switching animations
        public GameObject sa_happy_notification;

        public GameObject achievement_notification; //achievement complete notification
        public int achievement_noti_duration = 13;
        //cutscene declarations
        public float cutscene_delaytime = 5f;
        public float postcutscene_delay = 87f;
        public GameObject cutscene_vidplayer;

        //staff info declarations
        public bool staff_flag = false;
        public GameObject staff_info;

        //voice acting
        public float updatevolume = 0.02f;
        public AudioSource voiceline1;
        public AudioSource voiceline2;
        public AudioSource voiceline3;

        protected Coroutine m_DeactivationCoroutine;

        //big door reveal
        public static bool open = false;
        public AudioSource achievement_sound;
        public AudioSource bigdoormute;

        
        
        

        protected readonly int m_HashActivePara = Animator.StringToHash("Active");

        void Start()
        {
            client = new System.Net.Sockets.TcpClient();
            client.Connect("127.0.0.1", connectionPort);
            cutscene.SetActive(false);
            cutscene_vidplayer.SetActive(false);
        }

        IEnumerator SetAnimatorParameterWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            animator.SetBool(m_HashActivePara, false);
        }

        public void ReadStringInput(string s) //main input function 
        {
            input = s;
            Debug.Log(input);
            string user_input = input;
            Debug.Log(input.GetType());

            //inInfo = true;
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            //---Sending Data to Host----
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(user_input); //Converting string to byte data                                                           
            nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); //Sending the data in Bytes to Python

            if (s=="" || string.IsNullOrEmpty(s))
            {
                byte[] myWriteBuffer1 = Encoding.ASCII.GetBytes("empty data"); //Converting string to byte data                                                           
                nwStream.Write(myWriteBuffer1, 0, myWriteBuffer.Length);
                received_input = "enter valid data";
                return;
            }

            //---receiving Data from the Host----
            nwStream.Flush();
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize); //Getting data in Bytes from Python
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); //Converting byte data to string

            if (dataReceived != null)
            {
                //---Using received data---
                print(dataReceived + "rec");
                received_input = dataReceived;
            }

            //soundtrack switching animations
            //sad track
            if (dataReceived == "The forest was once a peaceful place, home to many leading simple and peaceful lives… but the monsters ruined everything. It was only a matter of months until people actively started to move out in fear, making this look like some form of wasteland.")
            {
                sa_sad_notification.SetActive(true);
                background_soundtrack.Stop();
                sad_soundtrack.Play();
                Destroy(sa_sad_notification, sa_animation_duration);
            }

            //happy track 
            if (dataReceived == "Thankfully the people all left in time and no one was harmed. They have now settled on the great plains and are living happily without worrying about the monsters!")
            {
                sa_happy_notification.SetActive(true);
                background_soundtrack.Stop();
                sad_soundtrack.Stop();
                happy_soundtrack.Play();
                Destroy(sa_happy_notification, sa_animation_duration);
                
            }

            //cutscene 
            if (dataReceived == "Okay then... this is going to be a long story so buckle in!")
            {
                StartCoroutine(delaycutscene());
                StartCoroutine(textaftercutscene());
                voiceline3.Play();
            }

            if (dataReceived == "It all started with the geo god and the stone dragon. There is a common folktale passed down generations that tell their story. Would you like to hear it?")
            {
                //happy_soundtrack.volume = updatevolume;
                //background_soundtrack.volume = updatevolume;
                voiceline2.Play();
                
            }
            //quest notification animations
            //start 
            if (dataReceived == "Im afraid all I know of is their henious deeds. But there is someone who may be able to tell you a lot more about these monsters. He looks like a knight out of some fantasy novel with some heavy armour. Try looking around the open courtyard, its really not that easy to miss him!")
            {
                quest_start_notification.SetActive(true);
                achievement_sound.Play();
                Destroy(quest_start_notification, questanimation_duration);
            }

            //end
            if (dataReceived == "Yeah sure… the monsters you are referring to are called the chroglodytes. They usually hunt in herds making it difficult to take them head on. What makes them even more dangerous is their poisonous teeth. However they are not even the main species you people should be concerned with… for theres a far bigger danger looming over the plains")
            {
               
                happy_soundtrack.volume = 0.1f;

                background_soundtrack.volume = 0.1f;
                voiceline1.Play();
                quest_end_notification.SetActive(true);
                achievement_sound.Play();
                Destroy(quest_end_notification, questanimation_duration);
   
            }

            //staff info
            if (dataReceived == "Yeah I do know a thing or two about the weapon you possess. Its an ancient staff called the staff of light. Perhaps this scroll could prvide you the answers that you are looking for.... (Press i to read scroll)")
            {
                staff_flag = true;

            }

            //achievement
            if (dataReceived == "My pleasure that I could be of help to you!")
            {
                StartCoroutine(achievement());
            }

        }


        public void ActivateCanvasWithText(string text)
        {
            if (m_DeactivationCoroutine != null)
            {
                StopCoroutine(m_DeactivationCoroutine);
                m_DeactivationCoroutine = null;
            }

            //received_input = "Hey there! My name is Sayu and Im the instructor that you were probably told to meet up with here. I know you are new here and must be curious about a lot of stuff … so if you have any questions, ask away! (Press r-shift to start conversing)";
            received_input = text;
            gameObject.SetActive(true);
            inInfo = true;
            animator.SetBool(m_HashActivePara, true);
            textMeshProUGUI.text = received_input;
        }

        public void Update()
        {
            //updating text inside infozone
            if (inInfo)
            {
                textMeshProUGUI.text = received_input;
            }

            //sa backdoor
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                sa_sad_notification.gameObject.SetActive(true);
                background_soundtrack.Stop();
                sad_soundtrack.Play();
                Destroy(sa_sad_notification, sa_animation_duration);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                sa_happy_notification.gameObject.SetActive(true);
                sad_soundtrack.Stop();
                background_soundtrack.Stop();
                happy_soundtrack.Play();
                Destroy(sa_happy_notification, sa_animation_duration);
            }

            //staff info
            if (staff_flag)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {

                    staff_info.SetActive(!staff_info.activeSelf);
                }
            }

            if (received_input != "Yeah I do know a thing or two about the weapon you possess. Its an ancient staff called the staff of light. Perhaps this scroll could prvide you the answers that you are looking for.... (Press i to read scroll)")
            {
                staff_flag = false;
            }

           
        }



        //cutscene co-routines
        public IEnumerator delaycutscene()
        {
            yield return new WaitForSecondsRealtime(cutscene_delaytime);
            cutscene.SetActive(true);
            cutscene_vidplayer.SetActive(true);
            print("video play");
            background_soundtrack.Stop();
            happy_soundtrack.Stop();
            Destroy(cutscene, cutscene_duration);

        }

        //achievement coroutine
        public IEnumerator achievement()
        {
            
            open = true;
            int achievement_delaytime = 8;
            yield return new WaitForSecondsRealtime(achievement_delaytime);
            achievement_notification.SetActive(true);
            achievement_sound.Play();
            Destroy(achievement_notification, achievement_noti_duration);
            received_input = "Ive been keeping an eye on you for a while and you really seem passionate about the forest of Eden. As a reward, you can now access the teleporter, which can take you to a place called the underground.... Although Im not too sure you would want to go there ....";
            
        }
        public IEnumerator textaftercutscene()
        {
            yield return new WaitForSecondsRealtime(postcutscene_delay);
            received_input = ".... and thats how things transpired that have eventually led the mosters ruining everything. If you have the courage then go to the underground and learn how to fight one of these mosnsters... only then would you be a match for the stone dragon!";
            postcutscene_soundtrack.Play();
            optional_q_notification.SetActive(true);
            Destroy(optional_q_notification, questanimation_duration);
            bigdoormute.Stop();


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

        //stop inp
       
    }
}
