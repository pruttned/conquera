//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using Ale.Settings;
using System.IO;
using Microsoft.Xna.Framework;
using FMOD;

namespace Ale.Sound
{
    public sealed class MusicPlayer
    {
        private FMOD.Sound mMusic = null;
        private FMOD.Channel mMusicChannel = null;

        private SoundManager mSoundManager;

        private string mMusicDir;

        private int mPlaylistPos = -1;

        public List<string> mPlaylist = new List<string>();

        internal MusicPlayer(SoundManager soundManager, string musicDir)
        {
            mSoundManager = soundManager;
            mMusicDir = musicDir;
            AppSettingsManager.Default.AppSettingsCommitted += new AppSettingsManager.CommittedHandler(Default_AppSettingsCommitted);
            ReloadeSettings();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playList">relative to Music folder. Without txt extension. null == music</param>
        public void Play(string playList)
        {
            if (string.IsNullOrEmpty(playList)) throw new ArgumentNullException("playList");

            //load playlist
            mPlaylist.Clear();
            mPlaylistPos = -1;
            string fileName = Path.Combine(mMusicDir, playList + ".txt");
            using (StreamReader reader = new StreamReader(fileName))
            {
                string musicFile;
                while (null != (musicFile = reader.ReadLine()))
                {
                    mPlaylist.Add(Path.Combine(mMusicDir, musicFile));
                }
            }

            PlayNextSong();
        }

        public void Play()
        {
            SoundSettings settings = AppSettingsManager.Default.GetSettings<SoundSettings>();
            if (settings.MusicVolume >= SoundManager.MinVolume)
            {
                if (null != mMusicChannel)
                {
                    mMusicChannel.setPaused(false);
                }
                else
                {
                    if (0 != mPlaylist.Count)
                    {
                        mPlaylistPos = -1;
                        PlayNextSong();
                    }
                    else
                    {
                        Play("music");
                    }
                }
            }
        }

        public void Stop()
        {
            if (null != mMusic)
            {
                SoundManager.CheckError(mMusicChannel.stop(), true);
                mMusicChannel = null;
                SoundManager.CheckError(mMusic.release(), true);
                mMusic = null;
            }
        }

        public void Pause()
        {
            if (null != mMusicChannel)
            {
                SoundManager.CheckError(mMusicChannel.setPaused(true), true);
            }
        }

        internal void Update()
        {
            //it is not possible to release a song in a callback
            if (null != mMusicChannel)
            {
                bool isPlaying = true;
                RESULT result = mMusicChannel.isPlaying(ref isPlaying);
                if (result == RESULT.ERR_INVALID_HANDLE)
                {
                    isPlaying = false;
                }
                else
                {
                    SoundManager.CheckError(result);
                }
                if (!isPlaying)
                {
                    PlayNextSong();
                }
            }
        }

        private void PlayNextSong()
        {
            if (null != mMusicChannel)
            {
                mMusicChannel.stop();
                mMusic.release();
            }

            if (0 < mPlaylist.Count)
            {
                mPlaylistPos++;
                if (mPlaylistPos == mPlaylist.Count)
                {
                    mPlaylistPos = 0;
                }

                string fileName = mPlaylist[mPlaylistPos];

                SoundSettings settings = AppSettingsManager.Default.GetSettings<SoundSettings>();

                SoundManager.CheckError(mSoundManager.System.createSound(fileName, (FMOD.MODE._2D | FMOD.MODE.HARDWARE | FMOD.MODE.CREATESTREAM | FMOD.MODE.IGNORETAGS), ref mMusic));
                SoundManager.CheckError(mSoundManager.System.playSound(FMOD.CHANNELINDEX.FREE, mMusic, true, ref mMusicChannel));
                mMusicChannel.setPriority(0);
                mMusicChannel.setVolume(settings.MusicVolume);

                mMusicChannel.setPaused(false);
            }
        }

        private void Default_AppSettingsCommitted(IAppSettings settings)
        {
            if (settings is SoundSettings)
            {
                ReloadeSettings();
            }
        }

        private void ReloadeSettings()
        {
            SoundSettings settings = AppSettingsManager.Default.GetSettings<SoundSettings>();
            if (null != mMusicChannel)
            {
                SoundManager.CheckError(mMusicChannel.setVolume(MathHelper.Clamp(settings.MusicVolume, 0, 1)), true);
            }
            if (settings.MusicVolume < SoundManager.MinVolume)
            {
                Stop();
            }
            else
            {
                Play();
            }
        }
    }

}
