using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;

namespace DDA
{
    static class AudioManager
    {
        static Dictionary<string, SoundEntity> entities = new Dictionary<string, SoundEntity>();
        static Dictionary<string, MapMusic> mapTracks = new Dictionary<string, MapMusic>();

        static SoundPlayer soundPlayer = new SoundPlayer();
        static SoundPlayer musicPlayer = new SoundPlayer();

        public static void LoadSounds()
        {
            string[] soundFolders = Directory.GetDirectories(@"..\..\..\sounds\sfx");

            foreach (string folder in soundFolders)
            {
                SoundEntity entity = new SoundEntity(folder);
                entities.Add(entity.Name, entity);
            }
        }
        public static void LoadMusic()
        {
            string[] mapNames = Directory.GetDirectories(@"..\..\..\sounds\music");

            foreach (string mapName in mapNames)
            {
                MapMusic entity = new MapMusic(mapName);
                mapTracks.Add(entity.Name, entity);
            }
        }
        public static void Play(string entityName, string soundName)
        {
            entities[entityName].PlaySound(soundName, ref soundPlayer);
        }
        public static void StartTrack(string mapMusicName, string trackName)
        {
            mapTracks[mapMusicName].PlaySound(trackName, ref musicPlayer);
        }

        protected class Entity
        {
            public string Name { get; init; }
            public string BasePath { get; init; }

            public Entity(string name)
            {
                Name = GetFileName(name);
            }

            public void PlaySound(string soundName, ref SoundPlayer player)
            {
                player.SoundLocation = string.Format(BasePath, Name, soundName);
                player.Play();
            }
            static string GetFileName(string filePath)
            {
                return filePath.Split(@"\").Last().Split(".")[0];
            }
        }
        protected class SoundEntity : Entity
        {
            public SoundEntity(string name) : base(name) => BasePath = @"..\..\..\sounds\sfx\{0}\{1}.wav";
        }
        protected class MapMusic : Entity
        {
            public MapMusic(string name) : base(name) => BasePath = @"..\..\..\sounds\music\{0}\{1}.wav";
        }
    }
}
