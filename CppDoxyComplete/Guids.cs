// Guids.cs
// MUST match guids.h
using System;

namespace PopDragos.CppDoxyComplete
{
    static class GuidList
    {
        public const string guidCppDoxyCompletePkgString = "66281e1a-6cb9-45db-95df-8bb60af6bef3";
        public const string guidCppDoxyCompleteCmdSetString = "e6c43674-5eb9-4684-9f66-dcf1e93ab2e2";

        public static readonly Guid guidCppDoxyCompleteCmdSet = new Guid(guidCppDoxyCompleteCmdSetString);
    };
}