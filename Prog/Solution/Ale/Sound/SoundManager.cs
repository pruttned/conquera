//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
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
using Microsoft.Xna.Framework;
using System.IO;
using Ale.Tools;
using FMOD;
using System.Runtime.Serialization;
using Ale.Content;

namespace Ale.Sound
{
    public sealed class SoundManager : IDisposable, IFrameListener
    {
        private FMOD.System mSystem;
        private FMOD.ChannelGroup mMasterChannelGroup;
        private FMOD.ChannelGroup mSoundChannelGroup;

        internal static float MinVolume = 0.0001f;

        private bool mIsDisposed = false;
        private string mSoundDir;

        private Vector3 mListenerPosition = new Vector3();
        private FMOD.VECTOR mFmodListenerPosition = new FMOD.VECTOR();
        private Vector3 mListenerUp = new Vector3();
        private FMOD.VECTOR mFmodListenerUp;
        private Vector3 mListenerForward = new Vector3();
        private FMOD.VECTOR mFmodListenerForward;

        private FMOD.VECTOR mFmodVel = new VECTOR();

        public Vector3 ListenerPosition 
        {
            get { return mListenerPosition; }
            set 
            {
                mListenerPosition = value;
                mFmodListenerPosition.x = mListenerPosition.X;
                mFmodListenerPosition.y = mListenerPosition.Z;
                mFmodListenerPosition.z = mListenerPosition.Y;
            }
        }

        public Vector3 ListenerUp
        {
            get { return mListenerUp; }
            set
            {
                mListenerUp = value;
                mFmodListenerUp.x = mListenerUp.X;
                mFmodListenerUp.y = mListenerUp.Z;
                mFmodListenerUp.z = mListenerUp.Y;
            }
        }

        public Vector3 ListenerForward
        {
            get { return mListenerForward; }
            set
            {
                mListenerForward = value;
                mFmodListenerForward.x = mListenerForward.X;
                mFmodListenerForward.y = mListenerForward.Z;
                mFmodListenerForward.z = mListenerForward.Y;
            }
        }

        internal FMOD.System System
        { 
            get { return mSystem;  } 
        }

        /// <summary>
        /// Each sound (not a music) must be placed into this group
        /// </summary>
        internal FMOD.ChannelGroup SoundChannelGroup
        {
            get { return mSoundChannelGroup; }
        }

        internal bool SoundIsMuted {get; private set;}
        
        public MusicPlayer MusicPlayer { get; private set; }

        internal SoundManager(string dataDir)
        {
            if(string.IsNullOrEmpty(dataDir)) throw new ArgumentNullException("dataDir");

            mFmodListenerUp = new VECTOR();
            mFmodListenerUp.x = 0; mFmodListenerUp.y = 1; mFmodListenerUp.z = 0;
            mFmodListenerForward = new VECTOR();
            mFmodListenerForward.x = 0; mFmodListenerForward.y = 0; mFmodListenerForward.z = 1;

            SoundSettings settings = AppSettingsManager.Default.GetSettings<SoundSettings>();
            if (!Path.IsPathRooted(dataDir))
            {
                dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dataDir);
            }
            mSoundDir = Path.Combine(dataDir, "Sound");
            string musicDir = Path.Combine(dataDir, "Music");

            CheckError(FMOD.Factory.System_Create(ref mSystem));
            CheckError(mSystem.setSpeakerMode(SPEAKERMODE.STEREO));
            CheckError(mSystem.init(settings.MaxChannelCnt, FMOD.INITFLAGS.NORMAL, (IntPtr)null));
            CheckError(mSystem.getMasterChannelGroup(ref mMasterChannelGroup));
            CheckError(mSystem.createChannelGroup("sound", ref mSoundChannelGroup));
            CheckError(mMasterChannelGroup.addGroup(mSoundChannelGroup));
            CheckError(mMasterChannelGroup.addGroup(mSoundChannelGroup));
            AppSettingsManager.Default.AppSettingsCommitted += new AppSettingsManager.CommittedHandler(Default_AppSettingsCommitted);
            ReloadeSettings();

            MusicPlayer = new MusicPlayer(this, musicDir);
        }

        internal void SetPauseAll(bool pause)
        {
            CheckError(mMasterChannelGroup.setPaused(pause));
        }
        
        ~SoundManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                if (System != null)
                {
                    CheckError(mSoundChannelGroup.release());

                    CheckError(System.close());
                    CheckError(System.release());
                }

                mIsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        internal static void CheckError(FMOD.RESULT result)
        {
            CheckError(result, false);
        }

        internal static void CheckError(FMOD.RESULT result, bool ignoreInvalidHandle)
        {
            if (result != FMOD.RESULT.OK && !(ignoreInvalidHandle && RESULT.ERR_INVALID_HANDLE == result))
            {
                throw new SoundSubsystemException(string.Format("FMOD error!  {0} - {1}", result, FMOD.Error.String(result)));
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
            CheckError(mSoundChannelGroup.setVolume(MathHelper.Clamp(settings.SoundVolume, 0, 1)));

            if (settings.SoundVolume < SoundManager.MinVolume)
            {
                SoundIsMuted = true;
                mSoundChannelGroup.stop();
            }
            else
            {
                SoundIsMuted = false;
            }
        }

        void IFrameListener.BeforeUpdate(AleGameTime gameTime)
        {
        }

        void IFrameListener.AfterUpdate(AleGameTime gameTime)
        {

            System.set3DListenerAttributes(0, ref mFmodListenerPosition, ref mFmodVel, ref mFmodListenerForward, ref mFmodListenerUp);

            System.update();
            MusicPlayer.Update();
        }

        void IFrameListener.BeforeRender(AleGameTime gameTime)
        {
        }

        void IFrameListener.AfterRender(AleGameTime gameTime)
        {
        }
    }

    public class SoundEmitter
    {
     //   private Sound3d mSound;
        private FMOD.Channel mChannel = null;

        public bool IsPlaying
        {
            get
            {
                bool isPlaying = true;
                RESULT result = mChannel.isPlaying(ref isPlaying);
                if (result == RESULT.ERR_INVALID_HANDLE)
                {
                    return false;
                }
                else
                {
                    SoundManager.CheckError(result);
                }
                return isPlaying;
            }
        }

        public void Play(Sound2d sound)
        {
            Stop();
            //mSound = sound;
            sound.Play(ref mChannel, true);
            mChannel.setPriority(100);

            mChannel.setPaused(false);
        }
     
        private void Stop()
        {
            if (null != mChannel)
            {
                SoundManager.CheckError(mChannel.stop(), true);
            }
        }
    }

    public class SoundEmitter3d
    {
        private static FMOD.VECTOR Velocity;

        //   private Sound3d mSound;
        private FMOD.Channel mChannel = null;
        private Vector3 mPosition = Vector3.Zero;

        public Vector3 Position
        {
            get { return mPosition; }
            set
            {
                mPosition = value;

                UpdateChannelPosition();
            }
        }

        public bool IsPlaying
        {
            get
            {
                bool isPlaying = true;
                RESULT result = mChannel.isPlaying(ref isPlaying);
                if (result == RESULT.ERR_INVALID_HANDLE)
                {
                    return false;
                }
                else
                {
                    SoundManager.CheckError(result);
                }
                return isPlaying;
            }
        }

        static SoundEmitter3d()
        {
            Velocity = new FMOD.VECTOR();
            Velocity.x = 0;
            Velocity.y = 0;
            Velocity.z = 0;
        }

        ///// <summary>
        ///// Chaning sound will stop the currently playing sound
        ///// </summary>
        //public AleSound Sound
        //{
        //    get { return mSound; }
        //    set
        //    {
        //        if (null == value) throw new ArgumentNullException("Sound");

        //        mSound = value;
        //        Stop();
        //    }
        //}

        //public SoundEmitter()
        //{
        //    if (null == sound) throw new ArgumentNullException("sound");
        //    mSound = sound;
        //}

        public void Play(Sound3d sound)
        {
            Stop();
            //mSound = sound;
            sound.Play(ref mChannel, true);
            UpdateChannelPosition();
            mChannel.setPriority(100);

            mChannel.setPaused(false);
        }
        //public void Play(bool loop)
        //{
        //    Stop();

        //    mSound.Play(ref mChannel, true);

        //    if (loop)
        //    {
        //        SoundManager.CheckError(mChannel.setMode(MODE.LOOP_NORMAL));
        //    }
        //    mChannel.setPriority(100);

        //    mChannel.setPaused(false);
        //}

        private void Stop()
        {
            if (null != mChannel)
            {
                SoundManager.CheckError(mChannel.stop(), true);
            }
        }

        private void UpdateChannelPosition()
        {
            if (null != mChannel)
            {
                FMOD.VECTOR fmodPos = new FMOD.VECTOR();
                fmodPos.x = mPosition.X;
                fmodPos.y = mPosition.Z;
                fmodPos.z = mPosition.Y;

                SoundManager.CheckError(mChannel.set3DAttributes(ref fmodPos, ref Velocity), true);
            }
        }
    }


    //public class AleSoundInstance
    //{
    //    private AleSound mSound;
    //    private FMOD.Channel mChannel;

    //    public AleSoundInstance(AleSound sound)
    //    {
    //        mSound = sound;
    //    }
    //}

    ////aux
    ////toto bude content - jedna sound sa da spustit viac krat..
    //public sealed class Sound : IDisposable
    //{
    //    SoundManager mSoundManager;
    //    private FMOD.Sound mSound = null;

    //    //aux musi byt v instancii prehravania a nie tu
    //    //private FMOD.Channel mChannel = null;

    //    private bool mIsDisposed = false;

    //    internal Sound(SoundManager soundManager, string fileName)
    //    {
    //        mSoundManager = soundManager;

    //        SoundManager.CheckError(mSoundManager.System.createSound(fileName, FMOD.MODE.HARDWARE, ref mSound));
    //    }

    //    public void Play()
    //    {
    //        FMOD.Channel channel = null;
    //        SoundManager.CheckError(mSoundManager.System.playSound(FMOD.CHANNELINDEX.FREE, mSound, false, ref channel));
    //    }

    //    ~Sound()
    //    {
    //        Dispose();
    //    }

    //    public void Dispose()
    //    {
    //        if (!mIsDisposed)
    //        {
    //            SoundManager.CheckError(mSound.release());

    //            mIsDisposed = true;
    //            GC.SuppressFinalize(this);
    //        }
    //    }
    //}
}
