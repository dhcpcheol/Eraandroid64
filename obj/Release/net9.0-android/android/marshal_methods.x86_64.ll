; ModuleID = 'marshal_methods.x86_64.ll'
source_filename = "marshal_methods.x86_64.ll"
target datalayout = "e-m:e-p270:32:32-p271:32:32-p272:64:64-i64:64-f80:128-n8:16:32:64-S128"
target triple = "x86_64-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [38 x ptr] zeroinitializer, align 16

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [114 x i64] [
	i64 u0x02abedc11addc1ed, ; 0: lib_Mono.Android.Runtime.dll.so => 36
	i64 u0x0517ef04e06e9f76, ; 1: System.Net.Primitives => 20
	i64 u0x0581db89237110e9, ; 2: lib_System.Collections.dll.so => 5
	i64 u0x07dcdc7460a0c5e4, ; 3: System.Collections.NonGeneric => 4
	i64 u0x092266563089ae3e, ; 4: lib_System.Collections.NonGeneric.dll.so => 4
	i64 u0x09d144a7e214d457, ; 5: System.Security.Cryptography => 29
	i64 u0x0c59ad9fbbd43abe, ; 6: Mono.Android => 37
	i64 u0x10f6cfcbcf801616, ; 7: System.IO.Compression.Brotli => 15
	i64 u0x13f1e5e209e91af4, ; 8: lib_Java.Interop.dll.so => 35
	i64 u0x152a448bd1e745a7, ; 9: Microsoft.Win32.Primitives => 2
	i64 u0x16bf2a22df043a09, ; 10: System.IO.Pipes.dll => 17
	i64 u0x1a91866a319e9259, ; 11: lib_System.Collections.Concurrent.dll.so => 3
	i64 u0x1aac34d1917ba5d3, ; 12: lib_System.dll.so => 33
	i64 u0x1c753b5ff15bce1b, ; 13: Mono.Android.Runtime.dll => 36
	i64 u0x209375905fcc1bad, ; 14: lib_System.IO.Compression.Brotli.dll.so => 15
	i64 u0x20fab3cf2dfbc8df, ; 15: lib_System.Diagnostics.Process.dll.so => 11
	i64 u0x2174319c0d835bc9, ; 16: System.Runtime => 28
	i64 u0x224538d85ed15a82, ; 17: System.IO.Pipes => 17
	i64 u0x2407aef2bbe8fadf, ; 18: System.Console => 9
	i64 u0x27b410442fad6cf1, ; 19: Java.Interop.dll => 35
	i64 u0x2801845a2c71fbfb, ; 20: System.Net.Primitives.dll => 20
	i64 u0x2af298f63581d886, ; 21: System.Text.RegularExpressions.dll => 30
	i64 u0x2afc1c4f898552ee, ; 22: lib_System.Formats.Asn1.dll.so => 14
	i64 u0x2d169d318a968379, ; 23: System.Threading.dll => 31
	i64 u0x2f02f94df3200fe5, ; 24: System.Diagnostics.Process => 11
	i64 u0x2f2e98e1c89b1aff, ; 25: System.Xml.ReaderWriter => 32
	i64 u0x31195fef5d8fb552, ; 26: _Microsoft.Android.Resource.Designer.dll => 0
	i64 u0x3235427f8d12dae1, ; 27: lib_System.Drawing.Primitives.dll.so => 12
	i64 u0x32aa989ff07a84ff, ; 28: lib_System.Xml.ReaderWriter.dll.so => 32
	i64 u0x341abc357fbb4ebf, ; 29: lib_System.Net.Sockets.dll.so => 22
	i64 u0x434c4e1d9284cdae, ; 30: Mono.Android.dll => 37
	i64 u0x49e952f19a4e2022, ; 31: System.ObjectModel => 23
	i64 u0x4bdd328408382c75, ; 32: EraAndroid64.dll => 1
	i64 u0x4d55a010ffc4faff, ; 33: System.Private.Xml => 25
	i64 u0x4e32f00cb0937401, ; 34: Mono.Android.Runtime => 36
	i64 u0x51bb8a2afe774e32, ; 35: System.Drawing => 13
	i64 u0x526ce79eb8e90527, ; 36: lib_System.Net.Primitives.dll.so => 20
	i64 u0x54795225dd1587af, ; 37: lib_System.Runtime.dll.so => 28
	i64 u0x571c5cfbec5ae8e2, ; 38: System.Private.Uri => 24
	i64 u0x579a06fed6eec900, ; 39: System.Private.CoreLib.dll => 34
	i64 u0x57c542c14049b66d, ; 40: System.Diagnostics.DiagnosticSource => 10
	i64 u0x5a8f6699f4a1caa9, ; 41: lib_System.Threading.dll.so => 31
	i64 u0x5ae9cd33b15841bf, ; 42: System.ComponentModel => 8
	i64 u0x5db0cbbd1028510e, ; 43: lib_System.Runtime.InteropServices.dll.so => 26
	i64 u0x5eb8046dd40e9ac3, ; 44: System.ComponentModel.Primitives => 6
	i64 u0x5f36ccf5c6a57e24, ; 45: System.Xml.ReaderWriter.dll => 32
	i64 u0x622eef6f9e59068d, ; 46: System.Private.CoreLib => 34
	i64 u0x65ece51227bfa724, ; 47: lib_System.Runtime.Numerics.dll.so => 27
	i64 u0x6692e924eade1b29, ; 48: lib_System.Console.dll.so => 9
	i64 u0x6769505529b13dad, ; 49: EraAndroid64 => 1
	i64 u0x6872ec7a2e36b1ac, ; 50: System.Drawing.Primitives.dll => 12
	i64 u0x68fbbbe2eb455198, ; 51: System.Formats.Asn1 => 14
	i64 u0x6a4d7577b2317255, ; 52: System.Runtime.InteropServices.dll => 26
	i64 u0x71ad672adbe48f35, ; 53: System.ComponentModel.Primitives.dll => 6
	i64 u0x76ca07b878f44da0, ; 54: System.Runtime.Numerics.dll => 27
	i64 u0x7bef86a4335c4870, ; 55: System.ComponentModel.TypeConverter => 7
	i64 u0x7dfc3d6d9d8d7b70, ; 56: System.Collections => 5
	i64 u0x7e946809d6008ef2, ; 57: lib_System.ObjectModel.dll.so => 23
	i64 u0x7ecc13347c8fd849, ; 58: lib_System.ComponentModel.dll.so => 8
	i64 u0x812c069d5cdecc17, ; 59: System.dll => 33
	i64 u0x82df8f5532a10c59, ; 60: lib_System.Drawing.dll.so => 13
	i64 u0x84ae73148a4557d2, ; 61: lib_System.IO.Pipes.dll.so => 17
	i64 u0x87f6569b25707834, ; 62: System.IO.Compression.Brotli.dll => 15
	i64 u0x897a606c9e39c75f, ; 63: lib_System.ComponentModel.Primitives.dll.so => 6
	i64 u0x8b4ff5d0fdd5faa1, ; 64: lib_System.Diagnostics.DiagnosticSource.dll.so => 10
	i64 u0x8b8d01333a96d0b5, ; 65: System.Diagnostics.Process.dll => 11
	i64 u0x8d7b8ab4b3310ead, ; 66: System.Threading => 31
	i64 u0x8da188285aadfe8e, ; 67: System.Collections.Concurrent => 3
	i64 u0x903101b46fb73a04, ; 68: _Microsoft.Android.Resource.Designer => 0
	i64 u0x90393bd4865292f3, ; 69: lib_System.IO.Compression.dll.so => 16
	i64 u0x91a74f07b30d37e2, ; 70: System.Linq.dll => 18
	i64 u0x944077d8ca3c6580, ; 71: System.IO.Compression.dll => 16
	i64 u0x97b8c771ea3e4220, ; 72: System.ComponentModel.dll => 8
	i64 u0x97e144c9d3c6976e, ; 73: System.Collections.Concurrent.dll => 3
	i64 u0xa0d8259f4cc284ec, ; 74: lib_System.Security.Cryptography.dll.so => 29
	i64 u0xa5f1ba49b85dd355, ; 75: System.Security.Cryptography.dll => 29
	i64 u0xa763fbb98df8d9fb, ; 76: lib_Microsoft.Win32.Primitives.dll.so => 2
	i64 u0xaa443ac34067eeef, ; 77: System.Private.Xml.dll => 25
	i64 u0xaa52de307ef5d1dd, ; 78: System.Net.Http => 19
	i64 u0xac2af3fa195a15ce, ; 79: System.Runtime.Numerics => 27
	i64 u0xadc90ab061a9e6e4, ; 80: System.ComponentModel.TypeConverter.dll => 7
	i64 u0xae282bcd03739de7, ; 81: Java.Interop => 35
	i64 u0xae53579c90db1107, ; 82: System.ObjectModel.dll => 23
	i64 u0xb220631954820169, ; 83: System.Text.RegularExpressions => 30
	i64 u0xb7212c4683a94afe, ; 84: System.Drawing.Primitives => 12
	i64 u0xb81a2c6e0aee50fe, ; 85: lib_System.Private.CoreLib.dll.so => 34
	i64 u0xba48785529705af9, ; 86: System.Collections.dll => 5
	i64 u0xbb65706fde942ce3, ; 87: System.Net.Sockets => 22
	i64 u0xbd0e2c0d55246576, ; 88: System.Net.Http.dll => 19
	i64 u0xc0d928351ab5ca77, ; 89: System.Console.dll => 9
	i64 u0xc12b8b3afa48329c, ; 90: lib_System.Linq.dll.so => 18
	i64 u0xc50fded0ded1418c, ; 91: lib_System.ComponentModel.TypeConverter.dll.so => 7
	i64 u0xc519125d6bc8fb11, ; 92: lib_System.Net.Requests.dll.so => 21
	i64 u0xc5325b2fcb37446f, ; 93: lib_System.Private.Xml.dll.so => 25
	i64 u0xc5a0f4b95a699af7, ; 94: lib_System.Private.Uri.dll.so => 24
	i64 u0xcbd4fdd9cef4a294, ; 95: lib__Microsoft.Android.Resource.Designer.dll.so => 0
	i64 u0xcc2876b32ef2794c, ; 96: lib_System.Text.RegularExpressions.dll.so => 30
	i64 u0xcd10a42808629144, ; 97: System.Net.Requests => 21
	i64 u0xcf23d8093f3ceadf, ; 98: System.Diagnostics.DiagnosticSource.dll => 10
	i64 u0xd1180337ea4020e8, ; 99: lib_EraAndroid64.dll.so => 1
	i64 u0xd333d0af9e423810, ; 100: System.Runtime.InteropServices => 26
	i64 u0xd3651b6fc3125825, ; 101: System.Private.Uri.dll => 24
	i64 u0xdad05a11827959a3, ; 102: System.Collections.NonGeneric.dll => 4
	i64 u0xdbf9607a441b4505, ; 103: System.Linq => 18
	i64 u0xdd2b722d78ef5f43, ; 104: System.Runtime.dll => 28
	i64 u0xe2420585aeceb728, ; 105: System.Net.Requests.dll => 21
	i64 u0xe2e426c7714fa0bc, ; 106: Microsoft.Win32.Primitives.dll => 2
	i64 u0xe5434e8a119ceb69, ; 107: lib_Mono.Android.dll.so => 37
	i64 u0xe89a2a9ef110899b, ; 108: System.Drawing.dll => 13
	i64 u0xedc4817167106c23, ; 109: System.Net.Sockets.dll => 22
	i64 u0xf1c4b4005493d871, ; 110: System.Formats.Asn1.dll => 14
	i64 u0xf3ddfe05336abf29, ; 111: System => 33
	i64 u0xf4c1dd70a5496a17, ; 112: System.IO.Compression => 16
	i64 u0xfbf0a31c9fc34bc4 ; 113: lib_System.Net.Http.dll.so => 19
], align 16

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [114 x i32] [
	i32 36, i32 20, i32 5, i32 4, i32 4, i32 29, i32 37, i32 15,
	i32 35, i32 2, i32 17, i32 3, i32 33, i32 36, i32 15, i32 11,
	i32 28, i32 17, i32 9, i32 35, i32 20, i32 30, i32 14, i32 31,
	i32 11, i32 32, i32 0, i32 12, i32 32, i32 22, i32 37, i32 23,
	i32 1, i32 25, i32 36, i32 13, i32 20, i32 28, i32 24, i32 34,
	i32 10, i32 31, i32 8, i32 26, i32 6, i32 32, i32 34, i32 27,
	i32 9, i32 1, i32 12, i32 14, i32 26, i32 6, i32 27, i32 7,
	i32 5, i32 23, i32 8, i32 33, i32 13, i32 17, i32 15, i32 6,
	i32 10, i32 11, i32 31, i32 3, i32 0, i32 16, i32 18, i32 16,
	i32 8, i32 3, i32 29, i32 29, i32 2, i32 25, i32 19, i32 27,
	i32 7, i32 35, i32 23, i32 30, i32 12, i32 34, i32 5, i32 22,
	i32 19, i32 9, i32 18, i32 7, i32 21, i32 25, i32 24, i32 0,
	i32 30, i32 21, i32 10, i32 1, i32 26, i32 24, i32 4, i32 18,
	i32 28, i32 21, i32 2, i32 37, i32 13, i32 22, i32 14, i32 33,
	i32 16, i32 19
], align 16

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 8

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 8

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 u0x0000000000000000, ; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 8

; Functions

; Function attributes: memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 8, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 16

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { memory(write, argmem: none, inaccessiblemem: none) "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+crc32,+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+crc32,+cx16,+cx8,+fxsr,+mmx,+popcnt,+sse,+sse2,+sse3,+sse4.1,+sse4.2,+ssse3,+x87" "tune-cpu"="generic" }

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
