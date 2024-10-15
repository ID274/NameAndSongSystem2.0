
/* Author  : Ignacy | https://github.com/ID274
 * License : CC BY 4.0 https://creativecommons.org/licenses/by/4.0/
 * Purpose : This script handles the management of the name dictionary and text files for the name system.
 *           It is also in charge of handling the naming conventions and naming logic for the vikings.
 *           As it is a singleton it can be accessed from any script in the project.
 *           Current intent is for this script to be used when creating new vikings either 
 *           as starting vikings or new viking children, but to be as reusable as possible if the design changes.
 *           
 * Tip     : Use the GetFullNameQuickly method to get a full name as a tuple. See the TestViking script for an example.
 */

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class NamingManager : MonoBehaviour
{
    static public NamingManager Instance { get; private set; }

    [Header("Full Names Dictionary\n-This dictionary is used to store the full names of vikings. \n-It is used to prevent duplicate names.\n\n=== DO NOT MODIFY THE DICTIONARY VIA INSPECTOR ===")]
    [SerializeField] private Dictionary<string, string> fullnameRecord = new Dictionary<string, string>();

    [Header("Name and Title Lists\n-These lists are populated from the text files. \n-They are used to assign names and titles to vikings.\n\n=== DO NOT MODIFY THESE LISTS VIA INSPECTOR === \n-Use the text files instead.")]
    [SerializeField] private List<string> maleNames = new List<string>();
    [SerializeField] private List<string> femaleNames = new List<string>();
    [SerializeField] private List<string> titles = new List<string>();

    [Header("Default Name Lists\n-Lists of default names for the system to use if it can't find the files.\n-These can be safely modified via inspector.")]
    [SerializeField] private List<string> defaultMaleNames = new List<string>();
    [SerializeField] private List<string> defaultFemaleNames = new List<string>();
    [SerializeField] private List<string> defaultTitles = new List<string>();

    private string path1, path2, path3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // <--- This will prevent the object from being destroyed when a new scene is loaded.
        }
        else
        {
            Destroy(this);
        }

        //- The persistentDataPath ensures that the path of the text files is always the same.
        //- This is important because the path is used to save and load the text files.
        //- The path should point to: %userprofile%\AppData\LocalLow\<companyname>\<productname>
        //- Unity Docs: https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html

        string directoryPath = Application.persistentDataPath + "/TextFiles";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath); // <--- This will create the directory if it does not exist.
        }
        path1 = directoryPath + "/MaleNames.txt";
        path2 = directoryPath + "/FemaleNames.txt";
        path3 = directoryPath + "/Titles.txt";

        VerifyTextFiles();
        LoadTextFiles();
    }

    void VerifyTextFiles() // <--- This method creates text files if they do not exist. If missing, default names are used.
    {
        if (!File.Exists(path1))
        {
            File.WriteAllText(path1, string.Join("\n", defaultMaleNames));
        }
        if (!File.Exists(path2))
        {
            File.WriteAllText(path2, string.Join("\n", defaultFemaleNames));
        }
        if (!File.Exists(path3))
        {
            File.WriteAllText(path3, string.Join("\n", defaultTitles));
        }
    }
    private void LoadTextFiles() // <--- This method loads the text files into the lists.
    {
        //- The text files are loaded into the lists maleNames, femaleNames, and titles.
        //- The text files are loaded from the persistentDataPath.

        maleNames = File.ReadAllLines(path1).ToList();
        femaleNames = File.ReadAllLines(path2).ToList();
        titles = File.ReadAllLines(path3).ToList();
    }
    public void AssignTitle(string title) // <--- IMPORTANT!!!!!!! should also take a viking as a parameter when implemented.
    {
        //Add title to viking
    }

    public string ChooseFirstName(bool isMale)
    {
        string chosenName = "";
        switch (isMale)
        {
            case true:
                chosenName = maleNames[Random.Range(0, maleNames.Count)];
                break;
            case false:
                chosenName = femaleNames[Random.Range(0, femaleNames.Count)];
                break;
        }
        return chosenName;
    }// <--- Use this one for all vikings/children. This method will assign a first name to a viking.

    //public string ChooseLastName(bool isMale) //  <--- Use this one for assigning names for starting vikings (parentless). This method will assign a last name to a viking.
    //{
    //    string chosenName = "";
    //    switch (isMale)
    //    {
    //        case true:
    //            chosenName = maleNames[Random.Range(0, maleNames.Count)] + "";
    //            break;
    //        case false:
    //            chosenName = femaleNames[Random.Range(0, femaleNames.Count)];
    //            break;
    //    }
    //    string surname = PatronymicSurname(chosenName, isMale);

    //    return surname;
    //}

    public string ChooseLastName(bool isMale, bool isChild) //  <--- isChild logic not implemented yet.
    {
        string chosenName = "";
        if (isChild)
        {
            //- Add logic for child last names. These should be based on the parents' last names.
        }
        switch (isMale)
        {
            case true:
                chosenName = maleNames[Random.Range(0, maleNames.Count)];
                break;
            case false:
                chosenName = femaleNames[Random.Range(0, femaleNames.Count)];
                break;
        }
        string surname = PatronymicSurname(chosenName, isMale);

        return surname;
    }

    private string PatronymicSurname(string surname, bool isMale)
    {
        //- This method creates a patronymic surname based on the name of the father.
        //- The implementation should take into account genitive form, and the suffixes -son (son) and -dottir (daughter).

        // NOT YET IMPLEMENTED, REQUIRES FURTHER RESEARCH INTO NAMING CONVENTIONS

        string nameSuffix = ""; // Placeholder
        
        string finalSurname = surname + nameSuffix;

        return finalSurname;
    }

    private (string, string) RecordFullName(bool isMale, bool isChild, string firstName, string lastName) // <--- This method records the full name of a viking and returns it as a tuple (string, string).
    {
        int counter = 0;
        while (true)
        {
            counter++;
            if (counter > 100)
            {
                Debug.LogError("Error: Unable to record full name. Dictionary length might be too low or available names exhausted.");
                return ("", ""); // <--- Ensure a return value here
            }

            if (counter > 1) // <--- This will reroll the first name if it is a duplicate.
            {
                firstName = ChooseFirstName(isMale); // <--- Choose a new first name and then check it against the same surname.

                if (fullnameRecord.TryGetValue(firstName, out string existingLastNameRerolled))
                {
                    if (existingLastNameRerolled == lastName)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    fullnameRecord.Add(firstName, lastName);
                    (string, string) fullName = (firstName, lastName); // <--- This will store the full name in a tuple.
                    Debug.Log($"Full name recorded: {fullName.ToString()}."); // <--- This will print the full name of the viking to the console.
                    return fullName;
                }
            }

            else
            {
                if (fullnameRecord.TryGetValue(firstName, out string existingLastName))
                {
                    if (existingLastName == lastName)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    fullnameRecord.Add(firstName, lastName);
                    (string, string) fullName = (firstName, lastName); // <--- This will store the full name in a tuple.
                    Debug.Log($"Full name recorded: {fullName.ToString()}."); // <--- This will print the full name of the viking to the console.
                    return fullName;
                }
            }
        }
        return ("", ""); // <--- Ensure a return value here
    }

    public (string, string) GetFullNameQuickly(bool isMale, bool isChild) // <--- This method is used to get a full name quickly. It returns a tuple (string, string).
    {
        string firstName = ChooseFirstName(isMale);
        string lastName = ChooseLastName(isMale, isChild);
        (string, string) fullName = RecordFullName(isMale, isChild, firstName, lastName);
        Debug.Log($"Full name: {fullName.ToString()}.");
        return fullName;
    }
}
