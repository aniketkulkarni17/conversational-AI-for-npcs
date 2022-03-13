import socket
import torch
from transformers import BertForQuestionAnswering
from transformers import BertTokenizer
import textwrap
from transformers import AutoTokenizer, AutoModelForSequenceClassification

HOST = '127.0.0.1'  # Standard loopback interface address (localhost)
PORT = 25001        # Port to listen on (non-privileged ports are > 1023)

model = BertForQuestionAnswering.from_pretrained(
    'bert-large-uncased-whole-word-masking-finetuned-squad')
tokenizer = BertTokenizer.from_pretrained(
    'bert-large-uncased-whole-word-masking-finetuned-squad')


def answer_question(question, answer_text):
    input_ids = tokenizer.encode(question, answer_text)

    # Report how long the input sequence is.
    print('Query has {:,} tokens.\n'.format(len(input_ids)))

    # ======== Set Segment IDs ========
    # Search the input_ids for the first instance of the `[SEP]` token.
    sep_index = input_ids.index(tokenizer.sep_token_id)

    # The number of segment A tokens includes the [SEP] token istelf.
    num_seg_a = sep_index + 1

    # The remainder are segment B.
    num_seg_b = len(input_ids) - num_seg_a

    # Construct the list of 0s and 1s.
    segment_ids = [0]*num_seg_a + [1]*num_seg_b

    # There should be a segment_id for every input token.
    assert len(segment_ids) == len(input_ids)

    # ======== Evaluate ========
    # Run our example through the model.
    outputs = model(torch.tensor([input_ids]),  # The tokens representing our input text.
                    # The segment IDs to differentiate question from answer_text
                    token_type_ids=torch.tensor([segment_ids]),
                    return_dict=True)

    start_scores = outputs.start_logits
    end_scores = outputs.end_logits

    # ======== Reconstruct Answer ========
    # Find the tokens with the highest `start` and `end` scores.
    answer_start = torch.argmax(start_scores)
    answer_end = torch.argmax(end_scores)

    # Get the string versions of the input tokens.
    tokens = tokenizer.convert_ids_to_tokens(input_ids)

    # Start with the first token.
    answer = tokens[answer_start]

    # Select the remaining answer tokens and join them with whitespace.
    for i in range(answer_start + 1, answer_end + 1):

        # If it's a subword token, then recombine it with the previous token.
        if tokens[i][0:2] == '##':
            answer += tokens[i][2:]

        # Otherwise, add a space then the token.
        else:
            answer += ' ' + tokens[i]

    print('Answer: "' + answer + '"')
    return answer


wrapper = textwrap.TextWrapper(width=80)

bert_abstract = "Ambers grandfather, a renowned mercenary leader from Liyue, falls victim to a disastrous monster attack that leaves him the sole survivor. After being saved by a doctor from the Knights of Favonius, he decides to move to Mondstadt and become a member himself. There, he established the Outriders, a division of archer scouts whose members he personally trained. He also starts a family there. He is implied to be the adventurer whom Granny luxoin fell in love with and promised to live with at Qingce Village, who embarked on an expedition only to disappear. The Qingce Village Bulletin Board contains a message about an unnamed former villager who went to Mondstadt to become a knight, who has recently (as of the present day) returned only to skulk about, though his manner of using a wind glider is apparently unmistakable."

print(wrapper.fill(bert_abstract))      

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen()
    conn, addr = s.accept()
    with conn:
        print('Connected by', addr)
        while True:
            data = conn.recv(1024).decode("UTF-8")
            if not data:
                response = "enter valid data"
                conn.sendall(response.encode("UTF-8"))     
       
            #genshin npc
            elif data == "Can you tell me a bit about yourself":
                response = "Me....? Im just a simple tavern owner. If you were to venture further inside the forest, you will surely notice my tavern, The Cats tail."
                conn.sendall(response.encode("UTF-8"))

            elif data == "Seems like your place is really busy. Mind telling me a bit more?":
                response = "Yeah sure… I dont like to admit it but the best tavern in the city is the Angels share. They have been getting a little too much attention as of late..."
                conn.sendall(response.encode("UTF-8"))
            
            elif data == "If that's the case then is it that much more difficult for you?":
                response = "Yeah it was difficult for a while...... However, ever since I hired Diona as my bartender, business is looking up. Doesnt really matter if they come for the wine or for her…. Money is flooding in now"
                conn.sendall(response.encode("UTF-8"))

            elif data == "Thank you for your time":
                response = "Anytime! If you ever want to have a little chat again, you know where to find me!"
                conn.sendall(response.encode("UTF-8"))

            elif data == "Do you remember what exactly happened in the forest?":
                response = "The forest was once a peaceful place, home to many leading simple and peaceful lives… but the monsters ruined everything. It was only a matter of months until people actively started to move out in fear, making this look like some form of wasteland."
                conn.sendall(response.encode("UTF-8"))
           
            elif data == "Do you have any update on the situation of the people who lived in the forest":
                response = "Thankfully the people all left in time and no one was harmed. They have now settled on the great plains and are living happily without worrying about the monsters!"
                conn.sendall(response.encode("UTF-8"))    
            
            #side quest    
            elif data == "Can you tell me more about those monsters": #quest initiate 
                response = "Im afraid all I know of is their henious deeds. But there is someone who may be able to tell you a lot more about these monsters. He looks like a knight out of some fantasy novel with some heavy armour. Try looking around the open courtyard, its really not that easy to miss him!"
                conn.sendall(response.encode("UTF-8")) 
            
            elif data == "Any advice on how to get to this underground place as quickly as possible":
                response = "Take the teleporting gate right across the open courtyard. You should get there in no time that way!"
                conn.sendall(response.encode("UTF-8"))

            elif data == "Can you tell me all that you know about the monsters":
                response = "Yeah sure… the monsters you are referring to are called the chroglodytes. They usually hunt in herds making it difficult to take them head on. What makes them even more dangerous is their poisonous teeth. However they are not even the main species you people should be concerned with… for theres a far bigger danger looming over the plains"
                conn.sendall(response.encode("UTF-8"))

            elif data == "What danger are you talking about":
                response = "It all started with the geo god and the stone dragon. There is a common folktale passed down generations that tell their story. Would you like to hear it?"
                conn.sendall(response.encode("UTF-8"))    

            elif data == "Yes I would like to hear it":
                response = "Okay then... this is going to be a long story so buckle in!"
                conn.sendall(response.encode("UTF-8"))    

            elif data == "Do you happen to know anything about this glowing stick I have":
                response = "Yeah I do know a thing or two about the weapon you possess. Its an ancient staff called the staff of light. Perhaps this scroll could prvide you the answers that you are looking for.... (Press i to read scroll)"
                conn.sendall(response.encode("UTF-8"))      

            elif data == "Can you tell me what is new in the forest of Eden":
                response = "Sure! As of your last time visitng, some of the music bugs have been squashed and the glowing problem with the staff of light seems to be fixed now! (For any updates you might have missed, simply ask what is new with the forest of eden!)"
                conn.sendall(response.encode("UTF-8")) 

            elif data == "Thank you for all the information":
                response = "My pleasure that I could be of help to you!"
                conn.sendall(response.encode("UTF-8")) 

            #qna reply
            else:
                print(data)
                question = data
                response = answer_question(question, bert_abstract) 
                conn.sendall(response.encode("UTF-8"))