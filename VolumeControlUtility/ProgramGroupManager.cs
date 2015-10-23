using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeControlUtility
{
    [Serializable]
    class ProgramGroupManager
    {
        public List<ProgramGroup> programGroups = new List<ProgramGroup>();
        public int NumOfProgramGroups = 0;

        public ProgramGroupManager()
        {

        }

        public void addProgramGroup(string toBeAdded)
        {
            ProgramGroup TBA = new ProgramGroup(toBeAdded, 100);
            TBA.updateVolume();
            programGroups.Add(TBA);
            updateNumOfGroups();
        }
        public void addProgramGroup(ProgramGroup toBeAdded)
        {
            toBeAdded.updateVolume();
            programGroups.Add(toBeAdded);
            updateNumOfGroups();
        }

        public ProgramGroup getProgramGroup(int index)
        {
            return programGroups.ElementAt(index);
        }
        public ProgramGroup getProgramGroup(string name)
        {
            foreach(ProgramGroup group in programGroups){
                if(group.getName() == name){
                    return group;
                }
            }
            return null;
        }

        public void removeProgramGroup(int IndexOfItemToBeRemoved)
        {
            programGroups.RemoveAt(IndexOfItemToBeRemoved);
            updateNumOfGroups();

        }

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
        public void updateNumOfGroups()
        {

            NumOfProgramGroups = programGroups.Count;
        }

        public int getNumOfProgramGroups()
        {
            return NumOfProgramGroups;
        }

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
