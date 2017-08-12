/*
** 
** electrifier
** 
** Copyright (c) 2017 Thorsten Jung @ electrifier.org and contributors
** 
*/

using System;
using System.Collections;
using System.Runtime.InteropServices;

using electrifier.Core.Services;
using electrifier.Core.WindowsShell.Services;
using electrifier.Win32API;

namespace electrifier.Core.WindowsShell
{
    /// <summary>
    /// Summary for BasicShellObjectCollection.
    /// </summary>

    public class BasicShellObjectCollection : IShellObjectCollection
    {
        protected static DesktopFolderInstance desktopFolderInstance = (DesktopFolderInstance)ServiceManager.Services.GetService(typeof(DesktopFolderInstance));
        protected BasicShellObject[] items = null;

        /// <summary>
        /// Creates a new collection of subitems of the folder identified by the given pidl.
        /// </summary>
        /// <param name="shellFolderPIDL"></param>
        public BasicShellObjectCollection(IntPtr shellFolderPIDL,
                                          ShellAPI.SHCONTF contentFlags,
                                          ShellObjectPIDLComparer.SortMode sortMode)
        {
            ShellAPI.IShellFolder shellFolder = desktopFolderInstance.GetIShellFolder(shellFolderPIDL);
            ShellAPI.IEnumIDList enumIdList = null;
            IntPtr relativePidl = IntPtr.Zero;
            ArrayList absolutePidls = new ArrayList();

            // TODO: Check for disc drives, show dialogbox when no disc in drive! => event!

            shellFolder.EnumObjects(IntPtr.Zero, contentFlags, ref enumIdList);
            // enumIdList.Reset(); TODO: Check, if SHCONTF_INIT_ON_FIRST_NEXT could help

            if (enumIdList == null)
            {
                this.items = new BasicShellObject[0];
            }
            else
            {
                // TODO: .Next() == 0 means .Next() = NOERROR
                while (enumIdList.Next(1, ref relativePidl, out int enumCount) == 0)
                {
                    if (enumCount == 1)
                    {
                        absolutePidls.Add(PIDLManager.Combine(shellFolderPIDL, relativePidl));

                        PIDLManager.Free(relativePidl);
                    }
                }

                if (sortMode != ShellObjectPIDLComparer.SortMode.Unsorted)
                    absolutePidls.Sort(new ShellObjectPIDLComparer(sortMode, shellFolder));

                // TODO: Try to optimize:
                this.items = new BasicShellObject[absolutePidls.Count];
                for (int i = 0; i < absolutePidls.Count; i++)
                    this.items[i] = new BasicShellObject((IntPtr)absolutePidls[i], true);

                Marshal.ReleaseComObject(enumIdList);
            }

            Marshal.ReleaseComObject(shellFolder);
        }

        #region IShellObjectCollection Member
        public int Count {
            get {
                return this.items.Length;
            }
        }

        public IShellObjectCollectionEnumerator GetEnumerator()
        {
            return new BasicShellObjectCollection.Enumerator(this);
        }
        #endregion

        #region Sub Class: Enumerator
        public class Enumerator : IShellObjectCollectionEnumerator
        {
            BasicShellObjectCollection collection;
            int currentIndex = -1;

            protected internal Enumerator(BasicShellObjectCollection basicShellObjectCollection)
            {
                this.collection = basicShellObjectCollection;
            }

            public bool MoveNext()
            {
                if (this.currentIndex > this.collection.items.Length)
                    throw new InvalidOperationException();

                return ++this.currentIndex < this.collection.items.Length;
            }

            public IShellObject Current {
                get {
                    if (this.currentIndex >= this.collection.items.Length)
                        throw new InvalidOperationException();

                    return this.collection.items[this.currentIndex];
                }
            }
        }
        #endregion
    }
}
