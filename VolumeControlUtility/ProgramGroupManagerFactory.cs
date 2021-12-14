using System;
using System.Linq;

namespace VolumeControlUtility
{
    public class ProgramGroupManagerFactory
    {
        AudioSessionManager ASM = new AudioSessionManager();
        
        public ProgramGroupManager getProgramGroupManager(ProgramGroupManagerData PGMdata)
        {
            ProgramGroupManager outPGM = new ProgramGroupManager();
            foreach (ProgramGroupData PGdata in PGMdata.programGroups)
            {
                if (PGdata.hasHotkey)
                {
                    outPGM.addProgramGroup(new ProgramGroup(PGdata.groupName, PGdata.volAsPercent, PGdata.mods, PGdata.volumeUp, PGdata.volumeDown) );
                }
                else
                {
                    outPGM.addProgramGroup(new ProgramGroup(PGdata.groupName, PGdata.volAsPercent));
                }
                
                
            }
            for (int programGroupIndex = 0; programGroupIndex < outPGM.getNumOfProgramGroups(); programGroupIndex++)
            {
                foreach (string strSession in PGMdata.programGroups.ElementAt(programGroupIndex).audioSessions)
                {
                    AudioSessionV2 aSession = ASM.getAudioSession(strSession);
                    if (aSession == null)
                    {
                        Console.WriteLine(strSession + " is not running, so it will not be loaded into an Program Group");
                        outPGM.programGroups.ElementAt(programGroupIndex).nonLoadedAudioSessions.Add(strSession);
                    }
                    else
                    {
                        outPGM.programGroups.ElementAt(programGroupIndex).addAudioSession(aSession, false);
                    }
                    //outPGM.programGroups.ElementAt(programGroupIndex).audioSessions.Add(strSession);

                }
            }
            return outPGM;
        }

    }
}
