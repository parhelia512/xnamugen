﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using xnaMugen.Elements;
using xnaMugen.Video;

namespace xnaMugen.Combat
{
    internal class TeamDisplay
    {
        public TeamDisplay(Team team)
        {
            if (team == null) throw new ArgumentNullException(nameof(team));

            m_team = team;
            m_combocounter = new ComboCounter(team);

            var textfile = m_team.Engine.GetSubSystem<IO.FileSystem>().OpenTextFile(@"data/fight.def");
            var lifebar = textfile.GetSection(GetLifebarSectionName(team.Mode));
            var powerbar = textfile.GetSection("Powerbar");
            var face = textfile.GetSection(GetFaceSectionName(team.Mode));
            var name = textfile.GetSection(GetNameSectionName(team.Mode));
            var winicon = textfile.GetSection("WinIcon");

            var prefix = Misc.GetPrefix(m_team.Side);
            var matePrefix = Misc.GetMatePrefix(m_team.Mode, m_team.Side);
            var elements = m_team.Engine.Elements;

            var lifebg0 = elements.Build(prefix + "lifebar.bg0", lifebar, prefix + ".bg0");
            var lifebg1 = elements.Build(prefix + "lifebar.bg1", lifebar, prefix + ".bg1");
            var lifemid = elements.Build(prefix + "lifebar.mid", lifebar, prefix + ".mid");
            var lifefront = elements.Build(prefix + "lifebar.front", lifebar, prefix + ".front");

            if (team.Mode == TeamMode.Simul)
            {
                var mateLifebg0 = elements.Build(matePrefix + "lifebar.bg0", lifebar, matePrefix + ".bg0");
                var mateLifebg1 = elements.Build(matePrefix + "lifebar.bg1", lifebar, matePrefix + ".bg1");
                var mateLifemid = elements.Build(matePrefix + "lifebar.mid", lifebar, matePrefix + ".mid");
                var mateLifefront = elements.Build(matePrefix + "lifebar.front", lifebar, matePrefix + ".front");
                var mateLifebarPosition = (Vector2) lifebar.GetAttribute<Point>(matePrefix + ".pos");
                var mateLifeBarRange = lifebar.GetAttribute<Point>(matePrefix + ".range.x");
                m_mateLifebar = new Lifebar(mateLifebg0, mateLifebg1, mateLifemid, mateLifefront, 
                    mateLifebarPosition, mateLifeBarRange);
            }

            m_powerbg0 = elements.Build(prefix + "powerbar.bg0", powerbar, prefix + ".bg0");
            m_powerbg1 = elements.Build(prefix + "powerbar.bg1", powerbar, prefix + ".bg1");
            m_powermid = elements.Build(prefix + "powerbar.mid", powerbar, prefix + ".mid");
            m_powerfront = elements.Build(prefix + "powerbar.front", powerbar, prefix + ".front");
            m_powercounter = elements.Build(prefix + "powerbar.counter", powerbar, prefix + ".counter");

            m_facebg = elements.Build(prefix + "face.bg", face, prefix + ".bg");
            m_faceimage = elements.Build(prefix + "face.face", face, prefix + ".face");

            if (team.Mode == TeamMode.Simul || team.Mode == TeamMode.Turns)
            {
                m_mateFaceBg = elements.Build(matePrefix + "face.bg", face, matePrefix + ".bg");
                m_mateFaceImage = elements.Build(matePrefix + "face.face", face, matePrefix + ".face");
                m_mateFaceKo = elements.Build(matePrefix + "face.ko", face, matePrefix + ".ko");
                m_mateFacePosition = (Vector2) face.GetAttribute<Point>(matePrefix + ".pos");
            }

            m_namelement = elements.Build(prefix + "name.name", name, prefix + ".name");

            if (team.Mode == TeamMode.Simul)
            {
                m_mateNameElement = elements.Build(matePrefix + "name.name", name, matePrefix + ".name");
                m_mateNamePosition = (Vector2) name.GetAttribute<Point>(matePrefix + ".pos");
            }

            m_winiconnormal = elements.Build(prefix + "winicon.normal", winicon, prefix + ".n");
            m_winiconspecial = elements.Build(prefix + "winicon.special", winicon, prefix + ".s");
            m_winiconhyper = elements.Build(prefix + "winicon.hyper", winicon, prefix + ".h");
            m_winiconthrow = elements.Build(prefix + "winicon.normalthrow", winicon, prefix + ".throw");
            m_winiconcheese = elements.Build(prefix + "winicon.cheese", winicon, prefix + ".c");
            m_winicontime = elements.Build(prefix + "winicon.timeout", winicon, prefix + ".t");
            m_winiconsuicide = elements.Build(prefix + "winicon.suicide", winicon, prefix + ".suicide");
            m_winiconteammate = elements.Build(prefix + "winicon.teammate", winicon, prefix + ".teammate");
            m_winiconperfect = elements.Build(prefix + "winicon.perfect", winicon, prefix + ".perfect");

            var lifebarposition = (Vector2) lifebar.GetAttribute<Point>(prefix + ".pos");
            var lifebarrange = lifebar.GetAttribute<Point>(prefix + ".range.x");

            m_powerbarposition = (Vector2) powerbar.GetAttribute<Point>(prefix + ".pos");
            m_powerbarrange = powerbar.GetAttribute<Point>(prefix + ".range.x");

            m_faceposition = (Vector2) face.GetAttribute<Point>(prefix + ".pos");
            m_nameposition = (Vector2) name.GetAttribute<Point>(prefix + ".pos");

            m_winiconposition = (Vector2) winicon.GetAttribute<Point>(prefix + ".pos");
            m_winiconoffset = (Vector2) winicon.GetAttribute<Point>(prefix + ".iconoffset");
            
            m_lifebar = new Lifebar(lifebg0, lifebg1, lifemid, lifefront, lifebarposition, lifebarrange);
        }

        private static string GetLifebarSectionName(TeamMode mode)
        {
            switch (mode)
            {
                case TeamMode.Simul:
                    return "Simul Lifebar";
                case TeamMode.Turns:
                    return "Turns Lifebar";
                default:
                    return "Lifebar";
            }
        }

        private static string GetFaceSectionName(TeamMode mode)
        {
            switch (mode)
            {
                case TeamMode.Simul:
                    return "Simul Face";
                case TeamMode.Turns:
                    return "Turns Face";
                default:
                    return "Face";
            }
        }

        private static string GetNameSectionName(TeamMode mode)
        {
            switch (mode)
            {
                case TeamMode.Simul:
                    return "Simul Name";
                case TeamMode.Turns:
                    return "Turns Name";
                default:
                    return "Name";
            }
        }

        public void Update()
        {
            if (m_team.Mode == TeamMode.Turns)
            {
                var player = m_team.OtherTeam.Wins.Count == 1 ? m_team.TeamMate : m_team.MainPlayer;
                m_lifebar.Update(player);
            }
            else
            {
                m_lifebar.Update(m_team.MainPlayer);
                m_mateLifebar?.Update(m_team.TeamMate);
            }
            ComboCounter.Update();
        }

        public void Draw()
        {
            if (m_team.Mode == TeamMode.Turns)
            {
                var player = m_team.OtherTeam.Wins.Count == 1 ? m_team.TeamMate : m_team.MainPlayer;
                m_lifebar.Draw(player);
            }
            else
            {
                m_lifebar.Draw(m_team.MainPlayer);
                m_mateLifebar?.Draw(m_team.TeamMate);
            }

            DrawPowerbar(m_team.MainPlayer);

            if (m_team.Mode == TeamMode.Turns)
            {
                var player = m_team.OtherTeam.Wins.Count == 1 ? m_team.TeamMate : m_team.MainPlayer;
                var matePlayer = m_team.OtherTeam.Wins.Count == 1 ? m_team.MainPlayer : m_team.TeamMate;
                DrawFace(player);
                DrawMateFace(matePlayer, m_team.OtherTeam.Wins.Count == 1);
            }
            else
            {
                DrawFace(m_team.MainPlayer);
                DrawMateFace(m_team.TeamMate, false);
            }

            
            if (m_team.Mode == TeamMode.Turns)
            {
                var player = m_team.OtherTeam.Wins.Count == 1 ? m_team.TeamMate : m_team.MainPlayer;
                var matePlayer = m_team.OtherTeam.Wins.Count == 1 ? m_team.MainPlayer : m_team.TeamMate;
                PrintName(player);
                PrintMateName(matePlayer);
            }
            else
            {
                PrintName(m_team.MainPlayer);
                PrintMateName(m_team.TeamMate);
            }

            DrawWinIcons();
        }

        private void DrawWinIcons()
        {
            var location = m_winiconposition;

            foreach (var win in m_team.Wins)
            {
                switch (win.Victory)
                {
                    case Victory.Normal:
                        m_winiconnormal.Draw(location);
                        break;

                    case Victory.Special:
                        m_winiconspecial.Draw(location);
                        break;

                    case Victory.Hyper:
                        m_winiconhyper.Draw(location);
                        break;

                    case Victory.NormalThrow:
                        m_winiconthrow.Draw(location);
                        break;

                    case Victory.Cheese:
                        m_winiconcheese.Draw(location);
                        break;

                    case Victory.Time:
                        m_winicontime.Draw(location);
                        break;

                    case Victory.Suicude:
                        m_winiconsuicide.Draw(location);
                        break;

                    case Victory.TeamKill:
                        m_winiconteammate.Draw(location);
                        break;
                }

                if (win.IsPerfectVictory) m_winiconperfect.Draw(location);

                location += m_winiconoffset;
            }
        }

        private void DrawPowerbar(Player player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));

            if (m_powerbg0.DataMap.Type == ElementType.Static || m_powerbg0.DataMap.Type == ElementType.Animation)
            {
                m_powerbg0.Draw(m_powerbarposition);
            }

            if (m_powerbg1.DataMap.Type == ElementType.Static || m_powerbg1.DataMap.Type == ElementType.Animation)
            {
                m_powerbg1.Draw(m_powerbarposition);
            }

            if (m_powermid.DataMap.Type == ElementType.Static || m_powermid.DataMap.Type == ElementType.Animation)
            {
                //m_powermid.Draw(m_powerbarposition);
            }

            if (m_powerfront != null && m_powerfront.DataMap.Type == ElementType.Static)
            {
                var powerPercentage = Math.Max(0.0f, player.Power / (float) player.Constants.MaximumPower);

                var drawstate = m_powerfront.SpriteManager.SetupDrawing(m_powerfront.DataMap.SpriteId,
                    m_powerbarposition, Vector2.Zero, m_powerfront.DataMap.Scale, m_powerfront.DataMap.Flip);
                drawstate.ScissorRectangle = DrawState.CreateBarScissorRectangle(m_powerfront, m_powerbarposition,
                    powerPercentage, m_powerbarrange);
                drawstate.Use();
            }

            if (m_powercounter.DataMap.Type == ElementType.Text)
            {
                var powertext = (player.Power / 1000).ToString();
                m_team.Engine.Print(m_powercounter.DataMap.FontData, m_powerbarposition + m_powercounter.DataMap.Offset,
                    powertext, null);
            }
        }

        private void DrawFace(Player player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));

            if (m_facebg.DataMap.Type == ElementType.Static || m_facebg.DataMap.Type == ElementType.Animation)
            {
                m_facebg.Draw(m_faceposition);
            }

            if (m_faceimage != null && m_faceimage.DataMap.Type == ElementType.Static)
            {
                var drawstate = player.SpriteManager.SetupDrawing(m_faceimage.DataMap.SpriteId,
                    m_faceposition + m_faceimage.DataMap.Offset, Vector2.Zero, m_faceimage.DataMap.Scale,
                    m_faceimage.DataMap.Flip);

                player.PaletteFx.SetShader(drawstate.ShaderParameters);

                drawstate.Use();
            }
        }

        private void DrawMateFace(Player player, bool isKo)
        {
            if (player == null) return;
            if (m_team.Mode == TeamMode.Single) return;

            if (m_mateFaceBg.DataMap.Type == ElementType.Static || m_mateFaceBg.DataMap.Type == ElementType.Animation)
            {
                m_mateFaceBg.Draw(m_mateFacePosition);
            }

            if (m_mateFaceImage != null && m_mateFaceImage.DataMap.Type == ElementType.Static)
            {
                var drawstate = player.SpriteManager.SetupDrawing(
                    m_mateFaceImage.DataMap.SpriteId, m_mateFacePosition + m_mateFaceImage.DataMap.Offset,
                    Vector2.Zero, m_mateFaceImage.DataMap.Scale, m_mateFaceImage.DataMap.Flip);

                player.PaletteFx.SetShader(drawstate.ShaderParameters);

                drawstate.Use();
            }

            if (!isKo) return;
            
            if (m_mateFaceKo.DataMap.Type == ElementType.Static || m_mateFaceKo.DataMap.Type == ElementType.Animation)
            {
                m_mateFaceKo.Draw(m_mateFacePosition);
            }
        }

        private void PrintName(Player player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));

            if (m_namelement.DataMap.Type == ElementType.Text)
            {
                m_team.Engine.Print(m_namelement.DataMap.FontData, m_nameposition, player.Profile.DisplayName, null);
            }
        }

        private void PrintMateName(Player player)
        {
            if (player == null) return;
            if (m_team.Mode == TeamMode.Turns) return;

            if (m_mateNameElement.DataMap.Type == ElementType.Text)
            {
                m_team.Engine.Print(m_mateNameElement.DataMap.FontData, m_mateNamePosition, player.Profile.DisplayName,
                    null);
            }
        }

        public ComboCounter ComboCounter => m_combocounter;

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Team m_team;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_powerbarposition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Point m_powerbarrange;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_faceposition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_nameposition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_winiconposition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_winiconoffset;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly ComboCounter m_combocounter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_powerbg0;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_powerbg1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_powermid;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_powerfront;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_powercounter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_facebg;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_faceimage;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_namelement;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_winiconnormal;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_winiconspecial;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_winiconhyper;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_winiconthrow;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_winiconcheese;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_winicontime;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_winiconsuicide;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_winiconteammate;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Base m_winiconperfect;

        private readonly Base m_mateFaceBg;
        private readonly Base m_mateFaceImage;
        private readonly Vector2 m_mateFacePosition;
        private readonly Base m_mateNameElement;
        private readonly Vector2 m_mateNamePosition;
        private readonly Base m_mateFaceKo;
        private readonly Lifebar m_lifebar;
        private readonly Lifebar m_mateLifebar;

        #endregion
    }
}