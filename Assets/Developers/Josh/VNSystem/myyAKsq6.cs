///////////////////////////////////////////////////////////////////////////////
//
// This code is a heavily modified version of work referenced at https://pastebin.com/myyAKsq6
// found through the youtube video https://www.youtube.com/watch?v=cmafUgj1cu8&t=534s
// changed to fit our projects backend
//
////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;


public class TwineParser
{
    public static List<DialogueTreeNode> ParseTwineText( string twineText )
        {
            List<DialogueTreeNode> output = new List<DialogueTreeNode>();
            string[] nodeData = twineText.Split(new string[] { "::" }, System.StringSplitOptions.None);
            DialogueTreeNode rootNode = null;
            bool passedHeader = false;
            //const int kIndexOfContentStart = 4;
            for ( int i = 0; i < nodeData.Length; i++ )
            {
            
                // The first node comes after the UserStylesheet node
                if ( !passedHeader )
                {
                    if ( nodeData[ i ].StartsWith( " StoryData" ) )
                        passedHeader = true;

                    continue;
                }
                // Note: tags are optional
                // Normal Format: "NodeTitle [Tags, comma, seperated] \r\n Message Text \r\n [[Response One]] \r\n [[Response Two]]"
                // No-Tag Format: "NodeTitle \r\n Message Text \r\n [[Response One]] \r\n [[Response Two]]"
                string currLineText = nodeData[i];

                // Remove position data
                int posBegin = currLineText.IndexOf("{\"position");
                if ( posBegin != -1 )
                {
                    int posEnd = currLineText.IndexOf("}", posBegin);
                    currLineText = currLineText.Substring( 0, posBegin ) + currLineText.Substring( posEnd + 1 );
                }
            
                bool tagsPresent = currLineText.IndexOf( "[" ) < currLineText.IndexOf( "\r\n" );
                int endOfFirstLine = currLineText.IndexOf( "\r\n" );
                if (endOfFirstLine == -1)
                {
                    endOfFirstLine = currLineText.IndexOf("\n");
                }

                //UnityEngine.Debug.Log( endOfFirstLine + ", " + tagsPresent );

                int startOfResponses = -1;
                int startOfResponseDestinations = currLineText.IndexOf( "[[" );
                bool lastNode = (startOfResponseDestinations == -1);
                if ( lastNode )
                    startOfResponses = currLineText.Length;
                else
                {
                    // Last new line before "[["
                    startOfResponses = currLineText.Substring( 0, startOfResponseDestinations ).LastIndexOf( "\r\n" );
                    if (startOfResponses == -1)
                    {
                        startOfResponses = currLineText.IndexOf("\n");
                    }
                //UnityEngine.Debug.Log(currLineText.Substring(0, startOfResponseDestinations).LastIndexOf( "\r\n" ));
            }
                // if (endOfFirstLine == -1)
                // {
                //     endOfFirstLine = currLineText.IndexOf(" ");
                // }
                // Extract Title
                int titleStart = 0;
                int titleEnd = tagsPresent
                    ? currLineText.IndexOf( "[" )
                    : endOfFirstLine;

                //UnityEngine.Assertions.Assert.IsTrue( titleEnd > 0, "Maybe you have a node with no responses?" );
                string title = "";
                if (titleEnd <=  0)
                {
                    title = GetFirstWordOfString(currLineText.Trim());
                    title.Trim();
                }
                else
                {
                    title = currLineText.Substring(titleStart, titleEnd).Trim();
                }

                // Extract Tags (if any)
                string tags = tagsPresent
                    ? currLineText.Substring( titleEnd + 1, (endOfFirstLine - titleEnd)-2)
                    : "";

                if (!string.IsNullOrEmpty(tags) && tags[tags.Length - 1] == ']')
                {
                    tags = tags.Substring(0, tags.Length - 1);
                }

                string messsageText = "";
                string responseText = "";
                // Extract Message Text & Responses
                if (endOfFirstLine > 0)
                {
                    messsageText = currLineText.Substring( endOfFirstLine, startOfResponses - endOfFirstLine).Trim();
                    responseText = currLineText.Substring( startOfResponses ).Trim();
                }
                //UnityEngine.Debug.Log(messsageText);

                DialogueTreeNode curNode = new DialogueTreeNode();
                //curNode.sceneData. = title;
                
                VisualNovelScene newSceneData = new VisualNovelScene();
                newSceneData.text = messsageText;
                curNode.sceneData = newSceneData;
                curNode.twineData.title = title;
                List<string> curTags = new List<string>(tags.Split(new string[] { " " }, System.StringSplitOptions.None));

                if (i == 0)
                {
                    UnityEngine.Assertions.Assert.IsTrue(null == rootNode);
                    rootNode = curNode;
                }

                //Note: response messages are optional(if no message then destination is the message)
                //     With Message Format: "\r\n Message[[Response One]]"
                //     Message - less Format: "\r\n [[Response One]]"
                //curNode.responses = new List<Response>();
                
                //UnityEngine.Debug.Log(responseText);

                if ( !lastNode )
                {
                    List<string> responseData = new List<string>(responseText.Split( new string [] { "\r\n" }, System.StringSplitOptions.None ));
                    if (responseData.Count == 1 && responseData[0] == responseText)
                    {
                        responseData = new List<string>(responseText.Split(new string[] { "\n" }, System.StringSplitOptions.None));
                    }
                //UnityEngine.Debug.Log(responseData);
                    foreach (string response in responseData)
                    {
                    //UnityEngine.Debug.Log(response);
                        curNode.twineData.responseData.Add(response);
                    }
                    // for ( int k = responseData.Count - 1; k >= 0; k-- )
                    // {
                        

                    //     string curResponseData = responseData[k];

                    //     if ( string.IsNullOrEmpty( curResponseData ) )
                    //     {
                    //         responseData.RemoveAt( k );
                    //         continue;
                    //     }
                    //     // curNode.children.Add(new DialogueTreeNode());
                    //     // curNode.children[i].parent = curNode;

                    //     curNode.twineData.responseData = curResponseData;

                    //     //Response curResponse = new Response();
                    //     int destinationStart = curResponseData.IndexOf( "[[" );
                    //     int destinationEnd = curResponseData.IndexOf( "]]" );
                    //     //UnityEngine.Assertions.Assert.IsFalse( destinationStart == -1, "No destination around in node titled, '" + curNode.title + "'" );
                    //     //UnityEngine.Assertions.Assert.IsFalse( destinationEnd == -1, "No destination around in node titled, '" + curNode.title + "'" );
                    //     string destination = curResponseData.Substring(destinationStart + 2, (destinationEnd - destinationStart)-2);
                    //     //curResponse.destinationNode = destination;
                    //     if (destinationStart == 0)
                    //         //curResponse.displayText = ""; // If message-less, then message is an empty string
                    //         curNode.children[i].sceneData.entryText = "";
                    //     else
                    //         //curResponse.displayText = curResponseData.Substring( 0, destinationStart );
                    //         //curNode.responses.Add(curResponse);
                    //         curNode.children[i].sceneData.entryText = curResponseData.Substring(0, destinationStart);
                    // }
                }
                output.Add(curNode);
                
                //nodes[ curNode.title ] = curNode;
            }
            return output;
        }

        public static DialogueTree ConstructTwineTree(List<DialogueTreeNode> nodes)
        {
            //DialogueTree output = new DialogueTree();
            DialogueTreeNode root = new DialogueTreeNode();
            for (int i = 0; i < nodes.Count; i++)
            {
                DialogueTreeNode node = nodes[i];
                if (i == 0)
                {
                    root = node;
                }
                for (int j = 0; j < node.twineData.responseData.Count; j++)
                {
                    string response = node.twineData.responseData[j];
                    UnityEngine.Debug.Log(response);
                    if (string.IsNullOrEmpty(response))
                    {
                        //node.twineData.responseData.Remove(response);
                        continue;
                        
                    }

                    int destinationStart = response.IndexOf("[[");
                    int destinationEnd = response.IndexOf("]]");

                    
                    UnityEngine.Assertions.Assert.IsFalse( destinationStart == -1, "No destination around in node titled, '" + node.twineData.title + "': " + destinationStart + ", " + destinationEnd );
                    UnityEngine.Assertions.Assert.IsFalse( destinationEnd == -1, "No destination around in node titled, '" + node.twineData.title + "'" );
                    string destination = response.Substring(destinationStart + 2, (destinationEnd - destinationStart)-2);
                    //curResponse.destinationNode = destination;

                    if (destinationStart != 0)
                    {
                        //int count = node.children.Count;
                        DialogueTreeNode responseNode = FindNodeWithTwineTitle(destination, ref nodes);
                        responseNode.sceneData.entryText = response;
                        responseNode.parent = node;
                        node.children.Add(responseNode);
                    }
                }
            }
            return new DialogueTree(root);
        }

        private static DialogueTreeNode FindNodeWithTwineTitle(string title, ref List<DialogueTreeNode> nodes)
        {
            foreach (DialogueTreeNode node in nodes)
            {
                if (node.twineData.title.Trim() == title)
                {
                    return node;
                }
            }
            UnityEngine.Debug.LogError("no node with name \"" + title + "\" was found");
            return null;
        }

        private static string GetFirstWordOfString(string text)
        {
            int EndOfWord = text.IndexOf(" ");
            string output;
            if (EndOfWord > 0)
            {
                output = text.Substring(0, EndOfWord);
            }
            else{
                output = text;
            }
            return output;
        }
}