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
using Ale.Content;
using Ale.Settings;
using Microsoft.Xna.Framework;

namespace Ale.Sound
{
    //[NonContentPipelineAsset(typeof(SoundLoader))]
    public abstract class AleSound : IDisposable
    {
        private static float SoundVolume;
        private FMOD.Sound mSound = null;
        private bool mIsDisposed = false;

        internal SoundManager SoundManager { get; private set; }

        static AleSound()
        {
            ReloadSettings();
        }

        /// <summary>
        /// !! Not protected - would be visible outside of assembly
        /// </summary>
        internal abstract FMOD.MODE Mode
        {
            get;
        }

        internal AleSound(SoundManager soundManager, string fileName)
        {
            SoundManager = soundManager;
            SoundManager.CheckError(soundManager.System.createSound(fileName, Mode, ref mSound));
        }

        /// <summary>
        /// !! Not protected - would be visible outside of assembly
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="paused"></param>
        internal void Play(ref FMOD.Channel channel, bool paused)
        {
            SoundManager.CheckError(SoundManager.System.playSound(FMOD.CHANNELINDEX.FREE, mSound, true, ref channel));
            channel.setPriority(100);
            channel.setChannelGroup(SoundManager.SoundChannelGroup);

            if (!paused)
            {
                channel.setPaused(false);
            }
        }

        ~AleSound()
        {
            Dispose(false);
        }
        
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                //if (isDisposing)
                //{
                //}
                SoundManager.CheckError(mSound.release());

                mIsDisposed = true;
            }
        }

        private static void ReloadSettings()
        {
            SoundSettings settings = AppSettingsManager.Default.GetSettings<SoundSettings>();
            SoundVolume = settings.SoundVolume;
        }
    }

    [NonContentPipelineAsset(typeof(SoundLoader2d))]
    public class Sound2d : AleSound
    {
        internal override FMOD.MODE Mode
        {
            get { return FMOD.MODE._2D | FMOD.MODE.HARDWARE | FMOD.MODE.IGNORETAGS; }
        }

        internal Sound2d(SoundManager soundManager, string fileName)
            : base(soundManager, fileName)
        {
        }

        public void Play()
        {
            if (!SoundManager.SoundIsMuted)
            {
                FMOD.Channel channel = null;
                Play(ref channel, false);
            }
        }
    }

    [NonContentPipelineAsset(typeof(SoundLoader3d))]
    public class Sound3d : AleSound
    {
        private static FMOD.VECTOR Velocity;

        internal override FMOD.MODE Mode
        {
            get { return FMOD.MODE._3D | FMOD.MODE.HARDWARE | FMOD.MODE.IGNORETAGS; }
            //get { return FMOD.MODE._3D | FMOD.MODE.HARDWARE | FMOD.MODE.IGNORETAGS | FMOD.MODE._3D_LINEARROLLOFF; }
        }

        static Sound3d()
        {
            Velocity = new FMOD.VECTOR();
            Velocity.x = 0;
            Velocity.y = 0;
            Velocity.z = 0;
        }

        internal Sound3d(SoundManager soundManager, string fileName)
            : base(soundManager, fileName)
        {
        }

        public void Play(ref Vector3 position)
        {
            if (!SoundManager.SoundIsMuted)
            {
                FMOD.Channel channel = null;
                Play(ref position, ref channel);
            }
        }

        internal void Play(Vector3 position)
        {
            Play(ref position);
        }

        public void Play(Vector3 position, ref FMOD.Channel channel)
        {
            Play(ref position, ref channel);
        }

        internal void Play(ref Vector3 position, ref FMOD.Channel channel)
        {
            if (!SoundManager.SoundIsMuted)
            {
                Play(ref channel, true);

                FMOD.VECTOR fmodPos = new FMOD.VECTOR();
                fmodPos.x = position.X;
                fmodPos.y = position.Z;
                fmodPos.z = position.Y;

              //  SoundManager.CheckError(channel.setMode(FMOD.MODE.LOOP_NORMAL));
                SoundManager.CheckError(channel.set3DAttributes(ref fmodPos, ref Velocity));

                SoundManager.CheckError(channel.set3DMinMaxDistance(3, 10000.0f));

                channel.setPaused(false);
            }
        }
    }
}
