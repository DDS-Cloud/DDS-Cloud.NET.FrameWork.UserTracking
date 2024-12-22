using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace DDS_Cloud.NET.FrameWork.UserTracking.Models
{
    internal class Win32API
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_INFO_0
        {
            public string UserName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct USER_INFO_11
        {
            public IntPtr usri11_name;
            public IntPtr usri11_comment;
            public uint usri11_flags;
            public IntPtr usri11_full_name;
            public IntPtr usri11_usr_comment;
            public IntPtr usri11_parms;
            public uint usri11_workstations;
            public uint usri11_logon_hours;
            public uint usri11_bad_pw_count;
            public uint usri11_num_logons;
            public IntPtr usri11_logon_server;
            public int usri11_country_code;
            public int usri11_code_page;
            public uint usri11_user_id;
            public uint usri11_primary_group_id;
            public IntPtr usri11_profile;
            public IntPtr usri11_home_dir_drive;
            public uint usri11_password_age;
            public IntPtr usri11_home_dir;
            public IntPtr usri11_logon_script;
            public IntPtr usri11_profile_path;
            public uint usri11_last_logon;
            public uint usri11_last_password_change;
            public uint usri11_password_expires;
            public uint usri11_account_expires;
            public uint usri11_user_flags;
            // Add other fields as needed
        }

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        public static extern int NetUserEnum(
            [MarshalAs(UnmanagedType.LPWStr)] string servername,
            int level,
            int filter,
            out IntPtr bufptr,
            int prefmaxlen,
            out int entriesread,
            out int totalentries,
            out int resume_handle);

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        public static extern int NetApiBufferFree(IntPtr Buffer);

        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        public static extern int NetUserGetInfo(
            [MarshalAs(UnmanagedType.LPWStr)] string servername,
            [MarshalAs(UnmanagedType.LPWStr)] string username,
            int level,
            out IntPtr bufptr);

        public static List<string> GetAllUsersOfSystem()
        {
            List<string> users = new List<string>();
            int EntriesRead;
            int TotalEntries;
            int Resume;
            IntPtr bufPtr;

            NetUserEnum(null, 0, 0, out bufPtr, -1, out EntriesRead, out TotalEntries, out Resume);

            if (EntriesRead > 0)
            {
                USER_INFO_0[] Users = new USER_INFO_0[EntriesRead];
                IntPtr iter = bufPtr;
                for (int i = 0; i < EntriesRead; i++)
                {
                    Users[i] = (USER_INFO_0)Marshal.PtrToStructure(iter, typeof(USER_INFO_0));
                    iter = (IntPtr)((int)iter + Marshal.SizeOf(typeof(USER_INFO_0)));
                    users.Add(Users[i].UserName);
                }
                NetApiBufferFree(bufPtr);
            }

            return users;
        }

        public static Dictionary<string, uint> GetUserIdsOfSystem()
        {
            Dictionary<string, uint> userIds = new Dictionary<string, uint>();
            int EntriesRead;
            int TotalEntries;
            int Resume;
            IntPtr bufPtr;

            NetUserEnum(null, 11, 0, out bufPtr, -1, out EntriesRead, out TotalEntries, out Resume);

            if (EntriesRead > 0)
            {
                USER_INFO_11[] Users = new USER_INFO_11[EntriesRead];
                IntPtr iter = bufPtr;
                for (int i = 0; i < EntriesRead; i++)
                {
                    Users[i] = (USER_INFO_11)Marshal.PtrToStructure(iter, typeof(USER_INFO_11));
                    iter = (IntPtr)((int)iter + Marshal.SizeOf(typeof(USER_INFO_11)));
                    string username = Marshal.PtrToStringUni(Users[i].usri11_name);
                    userIds.Add(username, Users[i].usri11_user_id);
                }
                NetApiBufferFree(bufPtr);
            }

            return userIds;
        }
    }
}