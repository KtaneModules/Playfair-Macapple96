using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using KMIntel;
using Newtonsoft.Json;

using Random = UnityEngine.Random;

public class playFair : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMBombModule module;
    public TextMesh ScreenText;
    //public KMSelectable Submit;
    public KMSelectable A, B, C, D;
    public TextMesh ALabel, BLabel, CLabel, DLabel;



    public KMAudio Audio;




    //ID stuff

    private static int _moduleIdCounter = 1;
    int _moduleId;

    string FKH = "";
    string SKH = "";
    string Key = "";
    string BufferKey = "";
    string Strikes = "";
    string FinalKey = "";
    string correctAns = "";
    string submittedAns;
    string[] possiblePrompts = { "STRIKE", "STROKE", "STRIFE", "STRIVE", "BLOWUP", "BLOWIN", "BLOWIT", "BLOWNX" };
    string[] possibleSolutions1 = { "ABCD", "BCDA", "CDAB", "DABC", "ABDC", "BDCA", "CABD", "DCAB" };
    string[] possibleSolutions2 = { "CDAB", "DACB", "ACBD", "CBDA", "BDAC", "DBCA", "BCAD", "CADB" };
    string[] possibleSolutions3 = { "BADC", "ADCB", "DCBA", "CBAD", "BACD", "ACDB", "CDBA", "DBAC" };
    string[] possibleSolutions4 = { "DABC", "ABCD", "BCDA", "CDAB", "DACB", "ACBD", "CBDA", "BDAC" };

    

    bool buttonsInteractable = false;
    bool solved = false;
    string answer;
    string encryptedAnswer;
    bool Alocked, Blocked, Clocked, Dlocked = false;
    string DOW = "";



    private Color Red = Color.red;
    private Color Yellow = new Color(1, 1, 0, 1);
    private Color Green = Color.green;
    private Color Blue = Color.blue;
    private Color Magenta = Color.magenta;
    private Color Orange = new Color(0.9f, 0.55f, 0.2f);

    private Color Black = Color.black;

    int textcolor;



    // Use this for initialization

    void Start()
    {

        _moduleId = _moduleIdCounter++;
        DOW = DateTime.Now.DayOfWeek.ToString();

        textcolor = Random.Range(0, 3);

        reroll();

        //DebugMsg("Key " + Key + " " + answer);

        module.OnActivate += Activate;



    }

    protected void reroll()
    {

        ALabel.color = Black;
        BLabel.color = Black;
        CLabel.color = Black;
        DLabel.color = Black;

        Alocked = false;
        Blocked = false;
        Clocked = false;
        Dlocked = false;

        A.transform.localPosition = new Vector3(-0.0514f, 0.01f, 0.015f);
        B.transform.localPosition = new Vector3(0.0126f, 0.01f, 0.015f);
        C.transform.localPosition = new Vector3(-0.0514f, 0.01f, -0.0486f);
        D.transform.localPosition = new Vector3(0.0126f, 0.01f, -0.0486f);

        submittedAns = "";
        FKH = "";
        SKH = "";
        Key = "";
        Strikes = "";

        startingTextColor();

        solution();

        getFirstKeyHalf();

        DebugMsg("Table 1 - First Key Half is: " + "\"" + FKH + "\"");

        getSecondKeyHalf();

        DebugMsg("Table 2 - Second Key Half is: " + "\"" + SKH + "\"");

        checkStrikes();

        checkVowels();

        appendKey();

        checkPrime();

        ClearDisplay();

        logMatrix(Key);

        

        buttonsInteractable = true;

    }

    void Activate()
    {
        module = GetComponent<KMBombModule>();



        A.OnInteract += delegate ()
        {
            if (!Alocked)
            {
                Alocked = true;
                A.transform.localPosition += new Vector3(0, -0.005f, 0);
                A.AddInteractionPunch();
                ALabel.color = Yellow;
                submittedAns = submittedAns + "A";
                HandlePress();
                return false;
            }
            else
            {
                return false;
            }

        };
        B.OnInteract += delegate ()
        {
            if (!Blocked)
            {
                Blocked = true;
                B.transform.localPosition += new Vector3(0, -0.005f, 0);
                B.AddInteractionPunch();
                BLabel.color = Yellow;
                submittedAns = submittedAns + "B";
                HandlePress();
                return false;
            }
            else
            {
                return false;
            }
        };
        C.OnInteract += delegate ()
        {
            if (!Clocked)
            {
                Clocked = true;
                C.transform.localPosition += new Vector3(0, -0.005f, 0);
                C.AddInteractionPunch();
                CLabel.color = Yellow;
                submittedAns = submittedAns + "C";
                HandlePress();
                return false;
            }
            else
            {
                return false;
            }
        };
        D.OnInteract += delegate ()
        {
            if (!Dlocked)
            {
                Dlocked = true;
                D.transform.localPosition += new Vector3(0, -0.005f, 0);
                D.AddInteractionPunch();
                DLabel.color = Yellow;
                submittedAns = submittedAns + "D";
                HandlePress();
                return false;
            }
            else
            {
                return false;
            }
        };

        buttonsInteractable = true;

        /*foreach (Button button in buttons) 
        {
        	Button realButton = button;
        	realButton.selectable.OnInteract += delegate () {buttonPress(realButton); return false;};
        }*/

        try
        {
            runPlayfair();
        }
        catch (Exception)
        {
            DebugMsg("Something went wrong with the encryption! Notify the author about this!");

            DebugMsg("Exception thrown, message is: " + answer + " Key is: " + Key);
            throw;

        }
    }

    void startingTextColor()
    {
        

        switch (textcolor)
        {
            case 0:

                ScreenText.color = Magenta;

                break;

            case 1:

                ScreenText.color = Blue;

                break;

            case 2:

                ScreenText.color = Orange;

                break;

            case 3:

                ScreenText.color = Yellow;

                break;

            default:

                ScreenText.color = Magenta;

                break;
        }
    }

    void getFirstKeyHalf()
    {
        bool isBobHere = Bomb.IsIndicatorOn(Indicator.BOB);
        if(isBobHere)
        {
            DebugMsg("BOB IS HOME!");
        }

        switch (DOW)
        {
            case "Monday":
                if(isBobHere) { FKH = "HIDDEN"; }
                else { FKH = "PLAY"; }
                
                break;

            case "Tuesday":
                FKH = "HIDDEN";

                break;

            case "Wednesday":
                if (isBobHere) { FKH = "CIPHER"; }
                else { FKH = "SECRET"; }

                break;

            case "Thursday":
                FKH = "CIPHER";
                break;

            case "Friday":
                if(isBobHere) { FKH = "PARTYHARD"; }
                else { FKH = "FAIL"; }
                
                break;

            case "Saturday":
                FKH = "PARTYHARD";

                break;

            case "Sunday":
                FKH = "BECOZY";

                break;

            default:
                FKH = "PLAY"; //If for some reason cannot fetch the DOW, default to monday
                break;
        }
    }

    void getSecondKeyHalf()
    {

        DebugMsg("textcolor: " + textcolor);

        string[] rule4 = { "MESSAGE", "EGASSEM", "SAFE", "EDOC" };
        string[] rule3 = { "GROOVE", "EVOORG", "TEIUQ", "QUITE" };
        string[] rule2 = { "CODE", "EDOC", "QUIET", "ETIUQ" };
        string[] rule1 = { "SAFE", "EFAS", "MESSAGE", "GROOVE" };

        int rule = 3;

        if(Bomb.GetBatteryCount(Battery.D) > Bomb.GetBatteryCount(Battery.AA)) //Rule 3
        {
            SKH = rule3[textcolor];
            DebugMsg("Table 2 - Rule #" + rule + " Applies: \"D Batteries > AA Batteries\"");
            rule--;
        }

        

        else if(Bomb.GetSerialNumberNumbers().Sum() > 10) //Rule 2
        {
            SKH = rule2[textcolor];
            DebugMsg("Table 2 - Rule #" + rule + " Applies: \"Sum of Digits in Serial > 10\"");
            rule--;
        }

        else if(Bomb.IsPortPresent(Port.Parallel) && Bomb.IsPortPresent(Port.Serial)) //Rule 1
        {
            SKH = rule1[textcolor];
            DebugMsg("Table 2 - Rule #" + rule + " Applies: \"Both Parallel and Serial Ports are present\"");
            rule--;
        }

        else
        {
            SKH = rule4[textcolor]; //Otherwise
            DebugMsg("Table 2 - Rule #4 \"No other rule applied\"");
        }
    }

    void appendKey()
    {
        Key = "";
        if (Bomb.GetStrikes() != 0)
        {
            Key = FKH + SKH + Strikes;
        }
        else { Key = FKH + SKH; }
        DebugMsg("Key now is: " + "\"" + Key + "\"");
    }

    void checkVowels()
    {
        if (!Bomb.GetSerialNumberLetters().Any("AEIOU".Contains))
        {
            DebugMsg("Table 3 - Serial doesn't contain a Vowel. Key 1: " + "\"" + FKH + "\"" + " | Key 2: " + "\"" + SKH + "\"");
            BufferKey = SKH;
            SKH = FKH;
            FKH = BufferKey;
            DebugMsg("Table 3 - Serial doesn't contain a Vowel, swapped Key 1: " + "\"" + FKH + "\"" + " and Key 2: " + "\"" + SKH + "\""); //I know this is not efficient way of swapping, I coul do it on appendKey(), but I like the log better this way.


        }

        else
        {
            DebugMsg("Table 3 - Serial contains a Vowel");
        }
    }

    void checkPrime()
    {
        if (new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31 }.Contains(Bomb.GetSerialNumberNumbers().Sum()))
        {
            char[] keyChars = Key.ToCharArray();
            Array.Reverse(keyChars);
            Key = new string(keyChars);
            DebugMsg("Table 5 - Sum of serial number numbers is " + Bomb.GetSerialNumberNumbers().Sum() + ", this number is prime!");
            DebugMsg("Key now is: " + "\"" + Key + "\"");
        }

        else
        {
            DebugMsg("Table 5 - Sum of serial number numbers is " + Bomb.GetSerialNumberNumbers().Sum() + ", this number is not prime.");
            DebugMsg("Key now is: " + "\"" + Key + "\"");
        }
    }

    void checkStrikes()
    {


        if (Bomb.GetStrikes() > 0)
        {
            if (Bomb.GetStrikes() == 1)
            {
                Strikes = "ONE";
            }
            else if (Bomb.GetStrikes() == 2)
            {
                Strikes = "TWO";
            }
            else
            {
                Strikes = "MANY";
            }
            DebugMsg("Table 4 - Bomb has " + Bomb.GetStrikes() + " (" + Strikes + ")" + " Strike(s)!");
            //appendKey();
        }

        else
        {
            DebugMsg("Table 4 - Bomb has 0 Strikes.");
        }
    }

    void HandlePress()
    {
        if (!solved)
        {


            if (!buttonsInteractable)
            {
                DebugMsg("Module is not active!");
                return;
            }

            if (submittedAns.Length < 4)
            {
                if (correctAns.StartsWith(submittedAns))
                {
                    DebugMsg("Submitted keys: " + submittedAns + ", so far so good!");
                    return;
                }
                else
                {
                    module.HandleStrike();
                    DebugMsg("Submitted keys: " + submittedAns + ", WRONG! STRIKE!");
                    buttonsInteractable = false;
                    DisplayWrong();
                    Invoke("reroll", 3);
                    Invoke("runPlayfair", 3);
                }


            }
            else
            {
                if (submittedAns == correctAns)
                {
                    module.HandlePass();
                    solved = true;
                    DisplayCorrect();
                }
                else
                {
                    DebugMsg("What the actual flock is happening, you should not see this! Notify the author!");
                }
            }
        }

        /*if (submittedAns != correctAns)
        {
            DebugMsg("Expected " + correctAns + ", input was " + "\"" + submittedAns + "\", Strike!");
            module.HandleStrike();
            DisplayWrong();

            Invoke("reroll", 3);
        }
        else
        {
            DebugMsg("Expected " + correctAns + ", input was " + "\"" + submittedAns + "\", Correct!");
            prompts++;
            module.HandlePass();
            DisplayCorrect();
            Invoke("ClearDisplay", 2);
        }*/

        Audio.PlaySoundAtTransform("tick", transform);
    }

    protected bool Solve()
    {
        module.OnPass();
        DisplayCorrect();
        buttonsInteractable = false;
        return false;
    }

    protected void Deactivation()
    {
        DebugMsg("Module deactivated"); //Why is this here again?

        ScreenText.text = "";
    }

    /// Playfair Enciphers given string
    void runPlayfair()
    {

        string encryptedAnswer = PlayfairCipher(Key, answer);
        DebugMsg("Encrypted prompt " + answer + " with key " + "\"" + Key + "\", result is " + "\"" + encryptedAnswer + "\"");

        /*///Disabled cause i'm a bad coder haha xd, wanted to change button position and playfair encipher "YEA" and "NAY" too.
		string yea = PlayfairCipher(inds, "YEA");
		string nay = PlayfairCipher(inds, "NAY");
		*/
        ScreenText.text = encryptedAnswer + "?";
        buttonsInteractable = true;
    }

    ///Logs a message
    void DebugMsg(string message)
    {
        Debug.LogFormat("[Playfair #{0}]: {1}", _moduleId, message);
    }



    void DisplayCorrect()
    {
        ALabel.color = Green;
        BLabel.color = Green;
        CLabel.color = Green;
        DLabel.color = Green;
        buttonsInteractable = false;
        ScreenText.text = "OKAY";
        ScreenText.color = Green;
    }

    void DisplayWrong()
    {
        ALabel.color = Red;
        BLabel.color = Red;
        CLabel.color = Red;
        DLabel.color = Red;
        buttonsInteractable = false;
        ScreenText.text = "WRONG";
        ScreenText.color = Red;
    }

    void ResetDisplay()
    {
        buttonsInteractable = true;
        ScreenText.text = "";
        
    }

    void ClearDisplay()
    {
        buttonsInteractable = false;
        ScreenText.text = "";
        
    }

    ///Get a prompt to be playfair enciphered, also pick the correct answer depending on the prompt.
    protected void solution()
    {

        var solutionnumber = Random.Range(0, possiblePrompts.Length);

        answer = possiblePrompts[solutionnumber];

        if (textcolor == 0) { correctAns = possibleSolutions1[solutionnumber]; DebugMsg("Message is " + "\"" + answer + "\", color is Magenta, expecting " + "\"" + correctAns + "\""); }
        if (textcolor == 1) { correctAns = possibleSolutions2[solutionnumber]; DebugMsg("Message is " + "\"" + answer + "\", color is Blue, expecting " + "\"" + correctAns + "\""); }
        if (textcolor == 2) { correctAns = possibleSolutions3[solutionnumber]; DebugMsg("Message is " + "\"" + answer + "\", color is Orange, expecting " + "\"" + correctAns + "\""); }
        if (textcolor == 3) { correctAns = possibleSolutions4[solutionnumber]; DebugMsg("Message is " + "\"" + answer + "\", color is Yellow, expecting " + "\"" + correctAns + "\""); }

    }



    void logMatrix(string key)
    {
        List<char> matrixout = new List<char>();
        var alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ"; //missing 'J' on purpose

        for (int i = 0; i < key.Length; i++)
        {
            if (!matrixout.Contains(key[i]))
            {
                matrixout.Add(key[i]);
            }
        }
        for (int i = 0; i < alphabet.Length; i++)
        {
            if (!matrixout.Contains(alphabet[i]))
            {
                matrixout.Add(alphabet[i]);
            }
        }
        var output = "";
        for (int i = 0; i < matrixout.Count; i++)
        {
            output += matrixout[i];
            if (i % 5 == 4)
            {
                output += "\n";
            }
        }
        DebugMsg("Output Matrix:\n" + output);

    }

    //Playfair

    //Declare struct CELL

    public struct Cell
    {
        public char character;
        public int X;
        public int Y;

        public Cell(char _character, int _X, int _Y)
        {
            this.character = _character;
            this.X = _X;
            this.Y = _Y;
        }
    }
    public string PlayfairCipher(string keyWord, string plainText)
    {
        //Debug.LogFormat("[Playfair #{2}] PlayfairCipher started – Encrypting text {1} with key {0}", keyWord, plainText, _moduleId);
        //Define alphabet
        //There is no J in the alphabet, I is used instead!
        char[] alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ".ToCharArray();

        //region Adjust Key
        keyWord = keyWord.Trim();
        keyWord = keyWord.Replace(" ", "");
        keyWord = keyWord.Replace("J", "I");
        keyWord = keyWord.ToUpper();

        StringBuilder keyString = new StringBuilder();

        foreach (char c in keyWord)
        {
            if (!keyString.ToString().Contains(c))
            {
                keyString.Append(c);
                alphabet = alphabet.Where(val => val != c).ToArray();
            }
        }
        //endregion

        adjustText(plainText);

        //If the Length of the plain text is odd add X
        if ((plainText.Length % 2 > 0))
        {
            plainText += "X";
        }

        List<string> plainTextEdited = new List<string>();

        //Split plain text into pairs
        for (int i = 0; i < plainText.Length; i += 2)
        {
            //If a pair of chars contains the same letters replace one of them with X
            if (plainText[i].ToString() == plainText[i + 1].ToString())
            {
                plainTextEdited.Add(plainText[i].ToString() + 'X');
            }
            else
            {
                plainTextEdited.Add(plainText[i].ToString() + plainText[i + 1].ToString());
            }
        }
        //endregion



        //region Create 5 x 5 matrix
        List<Cell> matrix = new List<Cell>();

        int keyIDCounter = 0;
        int alphabetIDCounter = 0;

        //Fill the matrix. First with the key characters then with the alphabet
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (keyIDCounter < keyString.Length)
                {

                    Cell cell = new Cell(keyString[keyIDCounter], x, y);
                    matrix.Add(cell);
                    keyIDCounter++;
                }
                else
                {
                    Cell cell = new Cell(alphabet[alphabetIDCounter], x, y);
                    matrix.Add(cell);
                    alphabetIDCounter++;

                }
                var curPos = "";

                for (int yG = 0; yG < 5; yG++)
                {
                    for (int xG = 0; xG < 5; xG++)
                    {
                        if (xG == 0) curPos += "[Playfair #" + _moduleId + "] ";

                        //curPos += " " + matrix[x, y];

                        if (xG == 5) curPos += "\n";

                    }
                }

            }
        }
        //endregion



        //region Write cipher

        StringBuilder cipher = new StringBuilder();

        foreach (string pair in plainTextEdited)
        {

            int indexA = matrix.FindIndex(c => c.character == pair[0]);
            Cell a = matrix[indexA];

            int indexB = matrix.FindIndex(c => c.character == pair[1]);
            Cell b = matrix[indexB];

            //Write cipher
            if (a.X == b.X)
            {
                cipher.Append(matrix[matrix.FindIndex(c => c.Y == (a.Y + 1) % 5 && c.X == a.X)].character);
                cipher.Append(matrix[matrix.FindIndex(c => c.Y == (b.Y + 1) % 5 && c.X == b.X)].character);
            }
            else if (a.Y == b.Y)
            {
                cipher.Append(matrix[matrix.FindIndex(c => c.Y == a.Y && c.X == (a.X + 1) % 5)].character);
                cipher.Append(matrix[matrix.FindIndex(c => c.Y == b.Y % 5 && c.X == (b.X + 1) % 5)].character);
            }
            else
            {
                cipher.Append(matrix[matrix.FindIndex(c => c.X == a.X && c.Y == b.Y)].character);
                cipher.Append(matrix[matrix.FindIndex(c => c.X == b.X % 5 && c.Y == a.Y)].character);
            }


        }
        //endregion
        //Debug.LogFormat ("[Playfair #{1}] – {0}", cipher.ToString(), _moduleId);
        return cipher.ToString();
    }


    //Make key Array
    private void baseKeyArray(string baseKey)
    {
        string baseKeyArray = baseKey;

        char[] baseCharArray = baseKeyArray.ToCharArray();

        foreach (var baseKeyArrayChar in baseCharArray)
        {
            //Debug.LogFormat ("[Playfair #{0}] Base Key Array: {1}", _moduleId, baseKeyArrayChar);
        }


    }

    //Remove Spaces, replace "J" with "I" and make UPPERCASE
    private static string adjustText(string text)
    {
        text = text.Trim();
        text = text.Replace(" ", "");
        text = text.Replace("J", "I");
        text = text.ToUpper();

        return text;
    }

    //If Text to Encrypt length is odd add "X"
    protected void checkOdd(string text)
    {
        bool wasOdd = false;
        if ((text.Length % 2 > 0))
        {
            text += "X";
            wasOdd = true;
        }

        Debug.LogFormat("[Playfair #{0}] Was the Text Odd?: {1}", _moduleId, wasOdd);

        getPairs(text);
    }

    //Split text into PAIRS
    private void getPairs(string textToPairs)
    {
        List<string> textEdit = new List<string>();
        for (int i = 0; i < textToPairs.Length; i += 2)
        {
            if (textToPairs[i].ToString() == textToPairs[i + 1].ToString())
            {
                textEdit.Add(textToPairs[i].ToString() + 'X');
            }
            else
            {
                textEdit.Add(textToPairs[i].ToString() + textToPairs[i + 1].ToString());
            }
        }

    }


#pragma warning disable 414
    private string TwitchHelpMessage = @"Submit your answer with “!{0} press A B C D”.";
#pragma warning restore 414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        var presses = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (presses.Length < 2 || (presses[0] != "submit" && presses[0] != "press"))
            yield break;
        else 
        {
            var buttons = new List<KMSelectable>();

            foreach (var press in presses.Skip(1))
            {
                if (press.Equals("A",StringComparison.InvariantCultureIgnoreCase))
                {
                    buttons.Add(A);
                }
                else if (press.Equals("B", StringComparison.InvariantCultureIgnoreCase))
                {
                    buttons.Add(B);
                }
                else if (press.Equals("C", StringComparison.InvariantCultureIgnoreCase))
                {
                    buttons.Add(C);
                }
                else if (press.Equals("D", StringComparison.InvariantCultureIgnoreCase))
                {
                    buttons.Add(D);
                }
                else
                {
                    //DebugMsg("BreakTwitchxd");
                    yield break;
                }
            }

            if (buttons.Count > 0)
            {
                yield return null;
                foreach (var bpress in buttons)
                {
                    //DebugMsg("xd");
                    bpress.OnInteract();
                    yield return new WaitForSeconds(.1f);
                }
            }
        }
    }

        //lights off room shown



        // Update is called once per frame
        void Update()
    {

    }
}