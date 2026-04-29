; ModuleID = 'compressed_assemblies.x86_64.ll'
source_filename = "compressed_assemblies.x86_64.ll"
target datalayout = "e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-f80:128-n8:16:32:64-S128"
target triple = "x86_64-unknown-linux-android21"

%struct.CompressedAssemblies = type {
	i32, ; uint32_t count
	ptr ; CompressedAssemblyDescriptor descriptors
}

%struct.CompressedAssemblyDescriptor = type {
	i32, ; uint32_t uncompressed_file_size
	i1, ; bool loaded
	ptr ; uint8_t data
}

@compressed_assemblies = dso_local local_unnamed_addr global %struct.CompressedAssemblies {
	i32 38, ; uint32_t count
	ptr @compressed_assembly_descriptors; CompressedAssemblyDescriptor* descriptors
}, align 8

@compressed_assembly_descriptors = internal dso_local global [38 x %struct.CompressedAssemblyDescriptor] [
	%struct.CompressedAssemblyDescriptor {
		i32 2560, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_0; uint8_t* data
	}, ; 0: _Microsoft.Android.Resource.Designer
	%struct.CompressedAssemblyDescriptor {
		i32 518144, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_1; uint8_t* data
	}, ; 1: EraAndroid64
	%struct.CompressedAssemblyDescriptor {
		i32 5120, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_2; uint8_t* data
	}, ; 2: Microsoft.Win32.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 17920, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_3; uint8_t* data
	}, ; 3: System.Collections.Concurrent
	%struct.CompressedAssemblyDescriptor {
		i32 11776, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_4; uint8_t* data
	}, ; 4: System.Collections.NonGeneric
	%struct.CompressedAssemblyDescriptor {
		i32 12288, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_5; uint8_t* data
	}, ; 5: System.Collections
	%struct.CompressedAssemblyDescriptor {
		i32 10752, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_6; uint8_t* data
	}, ; 6: System.ComponentModel.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 10752, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_7; uint8_t* data
	}, ; 7: System.ComponentModel.TypeConverter
	%struct.CompressedAssemblyDescriptor {
		i32 5120, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_8; uint8_t* data
	}, ; 8: System.ComponentModel
	%struct.CompressedAssemblyDescriptor {
		i32 11776, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_9; uint8_t* data
	}, ; 9: System.Console
	%struct.CompressedAssemblyDescriptor {
		i32 22016, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_10; uint8_t* data
	}, ; 10: System.Diagnostics.DiagnosticSource
	%struct.CompressedAssemblyDescriptor {
		i32 55808, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_11; uint8_t* data
	}, ; 11: System.Diagnostics.Process
	%struct.CompressedAssemblyDescriptor {
		i32 32256, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_12; uint8_t* data
	}, ; 12: System.Drawing.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 5120, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_13; uint8_t* data
	}, ; 13: System.Drawing
	%struct.CompressedAssemblyDescriptor {
		i32 59904, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_14; uint8_t* data
	}, ; 14: System.Formats.Asn1
	%struct.CompressedAssemblyDescriptor {
		i32 21504, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_15; uint8_t* data
	}, ; 15: System.IO.Compression.Brotli
	%struct.CompressedAssemblyDescriptor {
		i32 29184, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_16; uint8_t* data
	}, ; 16: System.IO.Compression
	%struct.CompressedAssemblyDescriptor {
		i32 22528, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_17; uint8_t* data
	}, ; 17: System.IO.Pipes
	%struct.CompressedAssemblyDescriptor {
		i32 24064, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_18; uint8_t* data
	}, ; 18: System.Linq
	%struct.CompressedAssemblyDescriptor {
		i32 125952, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_19; uint8_t* data
	}, ; 19: System.Net.Http
	%struct.CompressedAssemblyDescriptor {
		i32 57344, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_20; uint8_t* data
	}, ; 20: System.Net.Primitives
	%struct.CompressedAssemblyDescriptor {
		i32 7168, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_21; uint8_t* data
	}, ; 21: System.Net.Requests
	%struct.CompressedAssemblyDescriptor {
		i32 71680, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_22; uint8_t* data
	}, ; 22: System.Net.Sockets
	%struct.CompressedAssemblyDescriptor {
		i32 6144, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_23; uint8_t* data
	}, ; 23: System.ObjectModel
	%struct.CompressedAssemblyDescriptor {
		i32 67584, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_24; uint8_t* data
	}, ; 24: System.Private.Uri
	%struct.CompressedAssemblyDescriptor {
		i32 518656, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_25; uint8_t* data
	}, ; 25: System.Private.Xml
	%struct.CompressedAssemblyDescriptor {
		i32 9728, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_26; uint8_t* data
	}, ; 26: System.Runtime.InteropServices
	%struct.CompressedAssemblyDescriptor {
		i32 67072, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_27; uint8_t* data
	}, ; 27: System.Runtime.Numerics
	%struct.CompressedAssemblyDescriptor {
		i32 8192, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_28; uint8_t* data
	}, ; 28: System.Runtime
	%struct.CompressedAssemblyDescriptor {
		i32 120832, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_29; uint8_t* data
	}, ; 29: System.Security.Cryptography
	%struct.CompressedAssemblyDescriptor {
		i32 315392, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_30; uint8_t* data
	}, ; 30: System.Text.RegularExpressions
	%struct.CompressedAssemblyDescriptor {
		i32 12288, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_31; uint8_t* data
	}, ; 31: System.Threading
	%struct.CompressedAssemblyDescriptor {
		i32 5120, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_32; uint8_t* data
	}, ; 32: System.Xml.ReaderWriter
	%struct.CompressedAssemblyDescriptor {
		i32 4608, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_33; uint8_t* data
	}, ; 33: System
	%struct.CompressedAssemblyDescriptor {
		i32 1755136, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_34; uint8_t* data
	}, ; 34: System.Private.CoreLib
	%struct.CompressedAssemblyDescriptor {
		i32 160256, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_35; uint8_t* data
	}, ; 35: Java.Interop
	%struct.CompressedAssemblyDescriptor {
		i32 8192, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_36; uint8_t* data
	}, ; 36: Mono.Android.Runtime
	%struct.CompressedAssemblyDescriptor {
		i32 509440, ; uint32_t uncompressed_file_size
		i1 false, ; bool loaded
		ptr @__compressedAssemblyData_37; uint8_t* data
	} ; 37: Mono.Android
], align 16

@__compressedAssemblyData_0 = internal dso_local global [2560 x i8] zeroinitializer, align 16
@__compressedAssemblyData_1 = internal dso_local global [518144 x i8] zeroinitializer, align 16
@__compressedAssemblyData_2 = internal dso_local global [5120 x i8] zeroinitializer, align 16
@__compressedAssemblyData_3 = internal dso_local global [17920 x i8] zeroinitializer, align 16
@__compressedAssemblyData_4 = internal dso_local global [11776 x i8] zeroinitializer, align 16
@__compressedAssemblyData_5 = internal dso_local global [12288 x i8] zeroinitializer, align 16
@__compressedAssemblyData_6 = internal dso_local global [10752 x i8] zeroinitializer, align 16
@__compressedAssemblyData_7 = internal dso_local global [10752 x i8] zeroinitializer, align 16
@__compressedAssemblyData_8 = internal dso_local global [5120 x i8] zeroinitializer, align 16
@__compressedAssemblyData_9 = internal dso_local global [11776 x i8] zeroinitializer, align 16
@__compressedAssemblyData_10 = internal dso_local global [22016 x i8] zeroinitializer, align 16
@__compressedAssemblyData_11 = internal dso_local global [55808 x i8] zeroinitializer, align 16
@__compressedAssemblyData_12 = internal dso_local global [32256 x i8] zeroinitializer, align 16
@__compressedAssemblyData_13 = internal dso_local global [5120 x i8] zeroinitializer, align 16
@__compressedAssemblyData_14 = internal dso_local global [59904 x i8] zeroinitializer, align 16
@__compressedAssemblyData_15 = internal dso_local global [21504 x i8] zeroinitializer, align 16
@__compressedAssemblyData_16 = internal dso_local global [29184 x i8] zeroinitializer, align 16
@__compressedAssemblyData_17 = internal dso_local global [22528 x i8] zeroinitializer, align 16
@__compressedAssemblyData_18 = internal dso_local global [24064 x i8] zeroinitializer, align 16
@__compressedAssemblyData_19 = internal dso_local global [125952 x i8] zeroinitializer, align 16
@__compressedAssemblyData_20 = internal dso_local global [57344 x i8] zeroinitializer, align 16
@__compressedAssemblyData_21 = internal dso_local global [7168 x i8] zeroinitializer, align 16
@__compressedAssemblyData_22 = internal dso_local global [71680 x i8] zeroinitializer, align 16
@__compressedAssemblyData_23 = internal dso_local global [6144 x i8] zeroinitializer, align 16
@__compressedAssemblyData_24 = internal dso_local global [67584 x i8] zeroinitializer, align 16
@__compressedAssemblyData_25 = internal dso_local global [518656 x i8] zeroinitializer, align 16
@__compressedAssemblyData_26 = internal dso_local global [9728 x i8] zeroinitializer, align 16
@__compressedAssemblyData_27 = internal dso_local global [67072 x i8] zeroinitializer, align 16
@__compressedAssemblyData_28 = internal dso_local global [8192 x i8] zeroinitializer, align 16
@__compressedAssemblyData_29 = internal dso_local global [120832 x i8] zeroinitializer, align 16
@__compressedAssemblyData_30 = internal dso_local global [315392 x i8] zeroinitializer, align 16
@__compressedAssemblyData_31 = internal dso_local global [12288 x i8] zeroinitializer, align 16
@__compressedAssemblyData_32 = internal dso_local global [5120 x i8] zeroinitializer, align 16
@__compressedAssemblyData_33 = internal dso_local global [4608 x i8] zeroinitializer, align 16
@__compressedAssemblyData_34 = internal dso_local global [1755136 x i8] zeroinitializer, align 16
@__compressedAssemblyData_35 = internal dso_local global [160256 x i8] zeroinitializer, align 16
@__compressedAssemblyData_36 = internal dso_local global [8192 x i8] zeroinitializer, align 16
@__compressedAssemblyData_37 = internal dso_local global [509440 x i8] zeroinitializer, align 16

; Metadata
!llvm.module.flags = !{!0, !1}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/9.0.1xx @ 9abff7703206541fdb83ffa80fe2c2753ad1997b"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
