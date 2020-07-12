using System;
using System.Collections.Generic;
using System.Linq;

namespace VolumeControlUtility
{
    [Serializable]
    class ProgramGroupManager
    {
        public List<ProgramGroup> programGroups = new List<ProgramGroup>();
        public int NumOfProgramGroups = 0;

        /*
            Constructor
            */
        public ProgramGroupManager()
        {

        }

        /*
            Creates a Program Group with the specified name and adds
            the group to the Collection of Program Groups
            */
        public void createProgramGroup(string groupName)
        {
            ProgramGroup toBeAdded = new ProgramGroup(groupName, 100);
            toBeAdded.updateVolume();
            programGroups.Add(toBeAdded);
            updateNumOfGroups();
        }

        /*
            Adds a Program Group to the Collection
            */
        public void addProgramGroup(ProgramGroup toBeAdded)
        {
            toBeAdded.updateVolume();
            programGroups.Add(toBeAdded);
            updateNumOfGroups();
        }

        /*
            Remove a program group based on the index of the group
            in the Collection
            */
        public void removeProgramGroup(int IndexOfItemToBeRemoved)
        {
            programGroups.ElementAt(IndexOfItemToBeRemoved).unregisterHotkeys();
            programGroups.RemoveAt(IndexOfItemToBeRemoved);
            updateNumOfGroups();

        }

        /*
            Returns a specific program group based on the index of
            the program group in the Colleciton
            */
        public ProgramGroup getProgramGroup(int index)
        {
            return programGroups.ElementAt(index);
        }

        /*
            Returns a specific program group based on the name of
            the group stored in the Collection or returns null
            if not found
            */
        public ProgramGroup getProgramGroup(string name)
        {
            foreach(ProgramGroup group in programGroups){
                if(group.getName() == name){
                    return group;
                }
            }
            return null;
        }

        /*
            Prints the Collection of program groups to the 
            Console
            */
        public void displayProgramGroups()
        {
            Console.Clear();
            Console.WriteLine("======[Program Groups]======");
            // dump all audio sessions
            if (NumOfProgramGroups > 0)
            {
                for (int i = 0; i < programGroups.Count; i++)
                {
                    ProgramGroup currentGroup = programGroups.ElementAt(i);
                    currentGroup.updateActiveSessions();
                    Console.WriteLine(i + ") " + currentGroup.getName() + " - " + currentGroup.getNumOfPrograms() + " Programs");
                }
            }
            else
            {
                Console.WriteLine("You don't have any Program Groups!");
            }
            Console.WriteLine("======[END]======");
        }

        /*
            Updates the number of program groups stored
            in the Collection
            */
        public void updateNumOfGroups()
        {

            NumOfProgramGroups = programGroups.Count;
        }

        /*
            Returns the number of program groups stored
            in the Collection
            */
        public int getNumOfProgramGroups()
        {
            return NumOfProgramGroups;
        }

        /*
            Generates a serializable object for save data storage and persistance
            */
        public ProgramGroupManagerData generateProgramGroupManagerData()
        {
            ProgramGroupManagerData outPGMdata = new ProgramGroupManagerData();
            foreach(ProgramGroup group in programGroups){
                outPGMdata.programGroups.Add(group.generateProgramGroupData());
                outPGMdata.NumOfProgramGroups++;
            }
            return outPGMdata;
        }
    }
}
