; ModuleID = 'typemaps.arm64-v8a.ll'
source_filename = "typemaps.arm64-v8a.ll"
target datalayout = "e-m:e-i8:8:32-i16:16:32-i64:64-i128:128-n32:64-S128"
target triple = "aarch64-unknown-linux-android21"

%struct.TypeMapJava = type {
	i32, ; uint32_t module_index
	i32, ; uint32_t type_token_id
	i32 ; uint32_t java_name_index
}

%struct.TypeMapModule = type {
	[16 x i8], ; uint8_t module_uuid[16]
	i32, ; uint32_t entry_count
	i32, ; uint32_t duplicate_count
	ptr, ; TypeMapModuleEntry map
	ptr, ; TypeMapModuleEntry duplicate_map
	ptr, ; char* assembly_name
	ptr, ; MonoImage image
	i32, ; uint32_t java_name_width
	ptr ; uint8_t java_map
}

%struct.TypeMapModuleEntry = type {
	i32, ; uint32_t type_token_id
	i32 ; uint32_t java_map_index
}

@map_module_count = dso_local local_unnamed_addr constant i32 2, align 4

@java_type_count = dso_local local_unnamed_addr constant i32 152, align 4

; Managed modules map
@map_modules = dso_local local_unnamed_addr global [2 x %struct.TypeMapModule] [
	%struct.TypeMapModule {
		[16 x i8] [ i8 u0x14, i8 u0xaf, i8 u0xcb, i8 u0x31, i8 u0xf8, i8 u0xa6, i8 u0x01, i8 u0x43, i8 u0x96, i8 u0xf5, i8 u0xac, i8 u0xc7, i8 u0xa1, i8 u0x2b, i8 u0x69, i8 u0x24 ], ; module_uuid: 31cbaf14-a6f8-4301-96f5-acc7a12b6924
		i32 4, ; uint32_t entry_count
		i32 0, ; uint32_t duplicate_count
		ptr @module0_managed_to_java, ; TypeMapModuleEntry* map
		ptr null, ; TypeMapModuleEntry* duplicate_map
		ptr @.TypeMapModule.0_assembly_name, ; assembly_name: EraAndroid64
		ptr null, ; MonoImage* image
		i32 0, ; uint32_t java_name_width
		ptr null; uint8_t* java_map
	}, ; 0
	%struct.TypeMapModule {
		[16 x i8] [ i8 u0x2b, i8 u0x14, i8 u0x2b, i8 u0x30, i8 u0xcc, i8 u0x9b, i8 u0xc9, i8 u0x4e, i8 u0x93, i8 u0x5a, i8 u0x32, i8 u0x25, i8 u0x3f, i8 u0x0b, i8 u0xb6, i8 u0xc6 ], ; module_uuid: 302b142b-9bcc-4ec9-935a-32253f0bb6c6
		i32 148, ; uint32_t entry_count
		i32 51, ; uint32_t duplicate_count
		ptr @module1_managed_to_java, ; TypeMapModuleEntry* map
		ptr @module1_managed_to_java_duplicates, ; TypeMapModuleEntry* duplicate_map
		ptr @.TypeMapModule.1_assembly_name, ; assembly_name: Mono.Android
		ptr null, ; MonoImage* image
		i32 0, ; uint32_t java_name_width
		ptr null; uint8_t* java_map
	} ; 1
], align 8

; Java types name hashes
@map_java_hashes = dso_local local_unnamed_addr constant [152 x i64] [
	i64 u0x013d70f30586d278, ; 0 => javax/net/ssl/KeyManagerFactory
	i64 u0x01cd624f1e38cc9f, ; 1 => java/lang/Byte
	i64 u0x03cc98b851d4262c, ; 2 => javax/net/ssl/SSLContext
	i64 u0x0556c8677413cd01, ; 3 => java/lang/System
	i64 u0x06f84afe4273c430, ; 4 => java/net/InetSocketAddress
	i64 u0x083e83bb2321dd50, ; 5 => java/util/Random
	i64 u0x0ab77b7a4f03d9cf, ; 6 => android/widget/Adapter
	i64 u0x0b1da699fb29019a, ; 7 => android/os/BaseBundle
	i64 u0x0b95dc6056abf25b, ; 8 => android/widget/FrameLayout
	i64 u0x0c44130caa233945, ; 9 => mono/android/runtime/JavaObject
	i64 u0x0d9335f0988cd796, ; 10 => java/util/HashMap
	i64 u0x0fbd1a2d794a9718, ; 11 => android/widget/ListAdapter
	i64 u0x102731205d6f1f1c, ; 12 => android/graphics/Path
	i64 u0x106be7c89662702e, ; 13 => java/net/Proxy$Type
	i64 u0x10e015905ca8bd0f, ; 14 => java/security/cert/Certificate
	i64 u0x116532ec07ee0771, ; 15 => java/security/spec/KeySpec
	i64 u0x13638dc71e26c29f, ; 16 => android/graphics/Paint$Align
	i64 u0x13e5902d3b855db6, ; 17 => javax/net/ssl/TrustManagerFactory
	i64 u0x1759b71b41bc5f1b, ; 18 => android/content/pm/PackageItemInfo
	i64 u0x1b2b777b724ed302, ; 19 => crc64f7520a812a03248f/SetFontActivity
	i64 u0x1e04bf19f9c14045, ; 20 => android/util/AttributeSet
	i64 u0x1e549855226528a2, ; 21 => java/io/InterruptedIOException
	i64 u0x1e69018626ef9ffb, ; 22 => android/os/Handler
	i64 u0x21b381333982058e, ; 23 => javax/net/SocketFactory
	i64 u0x225c20a45cb91cd7, ; 24 => java/lang/Error
	i64 u0x228edb5145b4bbc1, ; 25 => android/view/InputEvent
	i64 u0x28c19c859a3f5d15, ; 26 => EraAndroid/FrontEnd/EmueraFrontEnd
	i64 u0x28cad0b9244cc1b5, ; 27 => android/widget/ListView
	i64 u0x2a15272bf231e341, ; 28 => android/widget/EditText
	i64 u0x2bcca4a8219ac237, ; 29 => javax/security/cert/X509Certificate
	i64 u0x2c0c9dbeeb804874, ; 30 => android/widget/AdapterView
	i64 u0x2cf8d24c3d21e911, ; 31 => android/text/TextPaint
	i64 u0x2ff9fb2c70f4f954, ; 32 => java/lang/SecurityException
	i64 u0x321c29cf8c6f7a93, ; 33 => android/content/res/Resources
	i64 u0x32d6a1d6ee9f6d5a, ; 34 => android/content/Intent
	i64 u0x332031975eda7654, ; 35 => java/lang/Boolean
	i64 u0x3436cf09b45d055e, ; 36 => java/security/Principal
	i64 u0x35e989807a64bcd9, ; 37 => java/lang/IllegalStateException
	i64 u0x406e54c64b3bee74, ; 38 => android/runtime/JavaProxyThrowable
	i64 u0x40c05cff47992547, ; 39 => android/view/ViewGroup
	i64 u0x41d091ef7039ab94, ; 40 => java/net/URLConnection
	i64 u0x4209344bc1b095c1, ; 41 => java/net/ProtocolException
	i64 u0x42e91d1f598314ca, ; 42 => android/database/DataSetObserver
	i64 u0x48e1abb584b78c94, ; 43 => java/io/Writer
	i64 u0x4a39213a97fe1b2f, ; 44 => java/net/ConnectException
	i64 u0x4a62077e41e01226, ; 45 => android/view/View$OnKeyListener
	i64 u0x4f9ad430b1a97fe6, ; 46 => android/widget/ScrollView
	i64 u0x5181b129b1a25949, ; 47 => java/lang/Class
	i64 u0x5238ad63b58da994, ; 48 => java/lang/ClassCastException
	i64 u0x529e559bd64e4c22, ; 49 => javax/net/ssl/HttpsURLConnection
	i64 u0x551ac881eb4466c0, ; 50 => java/lang/Number
	i64 u0x56365290d5a06704, ; 51 => java/lang/LinkageError
	i64 u0x57fe4a40460344db, ; 52 => android/os/Build$VERSION
	i64 u0x5a6af884fe3c181e, ; 53 => android/os/Bundle
	i64 u0x5b81bc1333f27da7, ; 54 => android/os/Environment
	i64 u0x5b905726d9bc975f, ; 55 => android/widget/TextView
	i64 u0x5bfd65ae1a6e6ffc, ; 56 => android/app/Activity
	i64 u0x5e1c513312ebc1b3, ; 57 => android/view/KeyEvent
	i64 u0x5e38b925960b7be9, ; 58 => android/graphics/Rect
	i64 u0x5f5a9fc3430795a4, ; 59 => android/content/ContextWrapper
	i64 u0x5f7e709faf8646e0, ; 60 => java/lang/Short
	i64 u0x6219335ac57fb821, ; 61 => java/io/Serializable
	i64 u0x6586889e8594dad8, ; 62 => android/widget/BaseAdapter
	i64 u0x65f6b14b7e978927, ; 63 => java/io/IOException
	i64 u0x6aa7d9af28b4551f, ; 64 => java/net/SocketTimeoutException
	i64 u0x6e0fb15bd0f04d15, ; 65 => java/lang/StackTraceElement
	i64 u0x6ef4975bdb7af18f, ; 66 => android/view/MotionEvent
	i64 u0x6ef7816e17e24358, ; 67 => android/graphics/Canvas
	i64 u0x714152b8b4c7f7d6, ; 68 => java/security/KeyFactory
	i64 u0x720cd712e1248c34, ; 69 => java/util/Iterator
	i64 u0x75591c18ddf5e52d, ; 70 => mono/android/TypeManager
	i64 u0x76cbd2104dd555ed, ; 71 => android/content/Context
	i64 u0x7b3aeb75b65cbd49, ; 72 => java/security/spec/PKCS8EncodedKeySpec
	i64 u0x7b925bdca68a0101, ; 73 => java/util/ArrayList
	i64 u0x7c93df30f68cf9a7, ; 74 => javax/security/auth/Subject
	i64 u0x7e23f87df6ff9d2a, ; 75 => mono/android/widget/AdapterView_OnItemLongClickListenerImplementor
	i64 u0x7f5865e567a08575, ; 76 => crc64f7520a812a03248f/SelectFolderActivity
	i64 u0x7fc6286783d5249d, ; 77 => java/security/Key
	i64 u0x7fd6b531797aa365, ; 78 => java/net/URL
	i64 u0x83314b5931a387fb, ; 79 => android/widget/Toast
	i64 u0x84a0e1080b630a71, ; 80 => android/util/TypedValue
	i64 u0x84f94178aab6cc34, ; 81 => java/lang/CharSequence
	i64 u0x888700b03d541d93, ; 82 => java/lang/RuntimeException
	i64 u0x88f7510c649f4a97, ; 83 => java/io/InputStream
	i64 u0x8a1927818aa18084, ; 84 => javax/net/ssl/KeyManager
	i64 u0x8a3ea3c274e8ce68, ; 85 => java/lang/Character
	i64 u0x8c9cbedbb1657afd, ; 86 => android/content/pm/ApplicationInfo
	i64 u0x90b4aeb45636cd6a, ; 87 => mono/android/runtime/OutputStreamAdapter
	i64 u0x92188d393e2af2d2, ; 88 => java/lang/Throwable
	i64 u0x92b59c839bc46278, ; 89 => java/lang/Thread
	i64 u0x965bfaf1ff1da014, ; 90 => java/lang/ReflectiveOperationException
	i64 u0x98ba110c6c57da31, ; 91 => java/lang/Float
	i64 u0x99df91bab800c287, ; 92 => mono/android/runtime/InputStreamAdapter
	i64 u0x9a23c2d41060f81e, ; 93 => java/io/File
	i64 u0x9e10a0b3efa170dc, ; 94 => android/view/ContextThemeWrapper
	i64 u0x9e6dc3e8eedaf8a8, ; 95 => java/net/SocketException
	i64 u0x9fa1370a1b1093fa, ; 96 => java/lang/NullPointerException
	i64 u0xa07cbd8408019386, ; 97 => java/net/Proxy
	i64 u0xa37a3a42636cca58, ; 98 => crc64f7520a812a03248f/MainActivity
	i64 u0xa59db4b8b7dbe046, ; 99 => javax/net/ssl/SSLSession
	i64 u0xa865adbdd81d9951, ; 100 => java/io/OutputStream
	i64 u0xa95eae500754348a, ; 101 => java/net/SocketAddress
	i64 u0xaa75ead031784774, ; 102 => javax/net/ssl/SSLSocketFactory
	i64 u0xabc3cd0f40f748aa, ; 103 => java/lang/String
	i64 u0xac7bbd754d805e27, ; 104 => android/graphics/BitmapFactory
	i64 u0xac9902bb0e4c5217, ; 105 => java/lang/IllegalArgumentException
	i64 u0xacbf549cdef93bef, ; 106 => java/net/HttpURLConnection
	i64 u0xb02badeb1c97535c, ; 107 => java/lang/Integer
	i64 u0xb18d71343ca8e96f, ; 108 => java/lang/Exception
	i64 u0xb4fc3e21cc054bc7, ; 109 => android/graphics/Paint
	i64 u0xb5ac04c19de8aabf, ; 110 => android/widget/AbsListView
	i64 u0xb69237f2a9d74c94, ; 111 => mono/android/view/View_OnKeyListenerImplementor
	i64 u0xb8df224d6b778ca3, ; 112 => android/view/View
	i64 u0xb9e48b25660487c5, ; 113 => javax/net/ssl/TrustManager
	i64 u0xbb84ccbe48f6c18b, ; 114 => android/os/Looper
	i64 u0xbf6d427143271cb3, ; 115 => java/lang/Object
	i64 u0xc00f4c2f11efdcff, ; 116 => java/lang/ClassNotFoundException
	i64 u0xc1a807325c15cf73, ; 117 => android/graphics/Bitmap
	i64 u0xc2a8e50a5f08afc6, ; 118 => mono/java/lang/RunnableImplementor
	i64 u0xc3eb0cbb47f178b9, ; 119 => java/lang/Enum
	i64 u0xc6dcfddd28ee4b89, ; 120 => mono/android/widget/AdapterView_OnItemClickListenerImplementor
	i64 u0xc9907bd32c160fff, ; 121 => android/util/Log
	i64 u0xc99e090e60d66f58, ; 122 => java/io/StringWriter
	i64 u0xca35caf567cfa745, ; 123 => java/util/Collection
	i64 u0xcc306823503920e9, ; 124 => android/app/Application
	i64 u0xd1b288a9c7bb8f53, ; 125 => java/lang/Double
	i64 u0xd5a28b8fa6d48e71, ; 126 => android/os/Build
	i64 u0xd7bf0ca2c70de05c, ; 127 => android/util/DisplayMetrics
	i64 u0xdd812f1d4afa427b, ; 128 => java/lang/UnsupportedOperationException
	i64 u0xde36efb42da7cc2d, ; 129 => javax/net/ssl/SSLSessionContext
	i64 u0xe024b538ad65ea66, ; 130 => java/util/function/Consumer
	i64 u0xe0446bf91fb0c2dd, ; 131 => java/lang/NoClassDefFoundError
	i64 u0xe1b3c5871398eb28, ; 132 => java/nio/channels/FileChannel
	i64 u0xe28cd0a2e6de00c1, ; 133 => java/security/KeyStore
	i64 u0xe59c130e7d1e4ac3, ; 134 => java/security/SecureRandom
	i64 u0xe66e73e6f14e03d2, ; 135 => android/widget/AdapterView$OnItemLongClickListener
	i64 u0xe70560eac84f8d02, ; 136 => android/widget/ArrayAdapter
	i64 u0xeb82145dcac4c559, ; 137 => java/lang/Long
	i64 u0xed49ed70aa9be1b3, ; 138 => java/nio/channels/spi/AbstractInterruptibleChannel
	i64 u0xee58348f4c4ad939, ; 139 => javax/net/ssl/HostnameVerifier
	i64 u0xee6f3d1e7507d907, ; 140 => java/util/Enumeration
	i64 u0xef2f2996a1d369cc, ; 141 => java/io/FileInputStream
	i64 u0xef953c41325a3428, ; 142 => java/io/PrintWriter
	i64 u0xf11f22a6441fcfbc, ; 143 => java/lang/IndexOutOfBoundsException
	i64 u0xf3d4ab08aaf25ccb, ; 144 => java/net/UnknownServiceException
	i64 u0xf85cbededb432844, ; 145 => java/security/spec/EncodedKeySpec
	i64 u0xfb0541dba11b69d9, ; 146 => android/graphics/Color
	i64 u0xfb9a51a22eb2843f, ; 147 => javax/security/cert/Certificate
	i64 u0xfbe9bfa5cc50fed6, ; 148 => java/util/HashSet
	i64 u0xfd2b1a3de667eb51, ; 149 => java/lang/Runnable
	i64 u0xfd830ff8e6ccffed, ; 150 => android/widget/AdapterView$OnItemClickListener
	i64 u0xfebf2b77f1940e7e ; 151 => java/security/PrivateKey
], align 8

@module0_managed_to_java = internal dso_local constant [4 x %struct.TypeMapModuleEntry] [
	%struct.TypeMapModuleEntry {
		i32 u0x02000219, ; uint32_t type_token_id
		i32 98; uint32_t java_map_index
	}, ; 0
	%struct.TypeMapModuleEntry {
		i32 u0x02000221, ; uint32_t type_token_id
		i32 76; uint32_t java_map_index
	}, ; 1
	%struct.TypeMapModuleEntry {
		i32 u0x02000223, ; uint32_t type_token_id
		i32 19; uint32_t java_map_index
	}, ; 2
	%struct.TypeMapModuleEntry {
		i32 u0x02000224, ; uint32_t type_token_id
		i32 26; uint32_t java_map_index
	} ; 3
], align 4

@module1_managed_to_java = internal dso_local constant [148 x %struct.TypeMapModuleEntry] [
	%struct.TypeMapModuleEntry {
		i32 u0x0200005f, ; uint32_t type_token_id
		i32 147; uint32_t java_map_index
	}, ; 0
	%struct.TypeMapModuleEntry {
		i32 u0x02000061, ; uint32_t type_token_id
		i32 29; uint32_t java_map_index
	}, ; 1
	%struct.TypeMapModuleEntry {
		i32 u0x02000063, ; uint32_t type_token_id
		i32 74; uint32_t java_map_index
	}, ; 2
	%struct.TypeMapModuleEntry {
		i32 u0x02000064, ; uint32_t type_token_id
		i32 23; uint32_t java_map_index
	}, ; 3
	%struct.TypeMapModuleEntry {
		i32 u0x02000066, ; uint32_t type_token_id
		i32 49; uint32_t java_map_index
	}, ; 4
	%struct.TypeMapModuleEntry {
		i32 u0x02000068, ; uint32_t type_token_id
		i32 139; uint32_t java_map_index
	}, ; 5
	%struct.TypeMapModuleEntry {
		i32 u0x0200006a, ; uint32_t type_token_id
		i32 84; uint32_t java_map_index
	}, ; 6
	%struct.TypeMapModuleEntry {
		i32 u0x0200006c, ; uint32_t type_token_id
		i32 99; uint32_t java_map_index
	}, ; 7
	%struct.TypeMapModuleEntry {
		i32 u0x0200006e, ; uint32_t type_token_id
		i32 129; uint32_t java_map_index
	}, ; 8
	%struct.TypeMapModuleEntry {
		i32 u0x02000070, ; uint32_t type_token_id
		i32 113; uint32_t java_map_index
	}, ; 9
	%struct.TypeMapModuleEntry {
		i32 u0x02000072, ; uint32_t type_token_id
		i32 0; uint32_t java_map_index
	}, ; 10
	%struct.TypeMapModuleEntry {
		i32 u0x02000073, ; uint32_t type_token_id
		i32 2; uint32_t java_map_index
	}, ; 11
	%struct.TypeMapModuleEntry {
		i32 u0x02000074, ; uint32_t type_token_id
		i32 102; uint32_t java_map_index
	}, ; 12
	%struct.TypeMapModuleEntry {
		i32 u0x02000076, ; uint32_t type_token_id
		i32 17; uint32_t java_map_index
	}, ; 13
	%struct.TypeMapModuleEntry {
		i32 u0x02000077, ; uint32_t type_token_id
		i32 42; uint32_t java_map_index
	}, ; 14
	%struct.TypeMapModuleEntry {
		i32 u0x02000079, ; uint32_t type_token_id
		i32 110; uint32_t java_map_index
	}, ; 15
	%struct.TypeMapModuleEntry {
		i32 u0x0200007c, ; uint32_t type_token_id
		i32 30; uint32_t java_map_index
	}, ; 16
	%struct.TypeMapModuleEntry {
		i32 u0x0200007d, ; uint32_t type_token_id
		i32 150; uint32_t java_map_index
	}, ; 17
	%struct.TypeMapModuleEntry {
		i32 u0x02000080, ; uint32_t type_token_id
		i32 120; uint32_t java_map_index
	}, ; 18
	%struct.TypeMapModuleEntry {
		i32 u0x02000081, ; uint32_t type_token_id
		i32 135; uint32_t java_map_index
	}, ; 19
	%struct.TypeMapModuleEntry {
		i32 u0x02000084, ; uint32_t type_token_id
		i32 75; uint32_t java_map_index
	}, ; 20
	%struct.TypeMapModuleEntry {
		i32 u0x0200008b, ; uint32_t type_token_id
		i32 136; uint32_t java_map_index
	}, ; 21
	%struct.TypeMapModuleEntry {
		i32 u0x0200008c, ; uint32_t type_token_id
		i32 55; uint32_t java_map_index
	}, ; 22
	%struct.TypeMapModuleEntry {
		i32 u0x0200008f, ; uint32_t type_token_id
		i32 62; uint32_t java_map_index
	}, ; 23
	%struct.TypeMapModuleEntry {
		i32 u0x02000091, ; uint32_t type_token_id
		i32 28; uint32_t java_map_index
	}, ; 24
	%struct.TypeMapModuleEntry {
		i32 u0x02000092, ; uint32_t type_token_id
		i32 8; uint32_t java_map_index
	}, ; 25
	%struct.TypeMapModuleEntry {
		i32 u0x02000093, ; uint32_t type_token_id
		i32 6; uint32_t java_map_index
	}, ; 26
	%struct.TypeMapModuleEntry {
		i32 u0x02000095, ; uint32_t type_token_id
		i32 11; uint32_t java_map_index
	}, ; 27
	%struct.TypeMapModuleEntry {
		i32 u0x02000097, ; uint32_t type_token_id
		i32 27; uint32_t java_map_index
	}, ; 28
	%struct.TypeMapModuleEntry {
		i32 u0x02000098, ; uint32_t type_token_id
		i32 46; uint32_t java_map_index
	}, ; 29
	%struct.TypeMapModuleEntry {
		i32 u0x02000099, ; uint32_t type_token_id
		i32 79; uint32_t java_map_index
	}, ; 30
	%struct.TypeMapModuleEntry {
		i32 u0x0200009b, ; uint32_t type_token_id
		i32 121; uint32_t java_map_index
	}, ; 31
	%struct.TypeMapModuleEntry {
		i32 u0x0200009d, ; uint32_t type_token_id
		i32 127; uint32_t java_map_index
	}, ; 32
	%struct.TypeMapModuleEntry {
		i32 u0x0200009e, ; uint32_t type_token_id
		i32 20; uint32_t java_map_index
	}, ; 33
	%struct.TypeMapModuleEntry {
		i32 u0x020000a0, ; uint32_t type_token_id
		i32 80; uint32_t java_map_index
	}, ; 34
	%struct.TypeMapModuleEntry {
		i32 u0x020000a1, ; uint32_t type_token_id
		i32 31; uint32_t java_map_index
	}, ; 35
	%struct.TypeMapModuleEntry {
		i32 u0x020000a2, ; uint32_t type_token_id
		i32 22; uint32_t java_map_index
	}, ; 36
	%struct.TypeMapModuleEntry {
		i32 u0x020000a3, ; uint32_t type_token_id
		i32 7; uint32_t java_map_index
	}, ; 37
	%struct.TypeMapModuleEntry {
		i32 u0x020000a4, ; uint32_t type_token_id
		i32 126; uint32_t java_map_index
	}, ; 38
	%struct.TypeMapModuleEntry {
		i32 u0x020000a5, ; uint32_t type_token_id
		i32 52; uint32_t java_map_index
	}, ; 39
	%struct.TypeMapModuleEntry {
		i32 u0x020000a7, ; uint32_t type_token_id
		i32 53; uint32_t java_map_index
	}, ; 40
	%struct.TypeMapModuleEntry {
		i32 u0x020000a8, ; uint32_t type_token_id
		i32 54; uint32_t java_map_index
	}, ; 41
	%struct.TypeMapModuleEntry {
		i32 u0x020000a9, ; uint32_t type_token_id
		i32 114; uint32_t java_map_index
	}, ; 42
	%struct.TypeMapModuleEntry {
		i32 u0x020000aa, ; uint32_t type_token_id
		i32 112; uint32_t java_map_index
	}, ; 43
	%struct.TypeMapModuleEntry {
		i32 u0x020000ab, ; uint32_t type_token_id
		i32 45; uint32_t java_map_index
	}, ; 44
	%struct.TypeMapModuleEntry {
		i32 u0x020000ae, ; uint32_t type_token_id
		i32 111; uint32_t java_map_index
	}, ; 45
	%struct.TypeMapModuleEntry {
		i32 u0x020000b2, ; uint32_t type_token_id
		i32 57; uint32_t java_map_index
	}, ; 46
	%struct.TypeMapModuleEntry {
		i32 u0x020000b3, ; uint32_t type_token_id
		i32 66; uint32_t java_map_index
	}, ; 47
	%struct.TypeMapModuleEntry {
		i32 u0x020000b4, ; uint32_t type_token_id
		i32 94; uint32_t java_map_index
	}, ; 48
	%struct.TypeMapModuleEntry {
		i32 u0x020000b6, ; uint32_t type_token_id
		i32 25; uint32_t java_map_index
	}, ; 49
	%struct.TypeMapModuleEntry {
		i32 u0x020000bb, ; uint32_t type_token_id
		i32 39; uint32_t java_map_index
	}, ; 50
	%struct.TypeMapModuleEntry {
		i32 u0x020000d1, ; uint32_t type_token_id
		i32 92; uint32_t java_map_index
	}, ; 51
	%struct.TypeMapModuleEntry {
		i32 u0x020000d3, ; uint32_t type_token_id
		i32 123; uint32_t java_map_index
	}, ; 52
	%struct.TypeMapModuleEntry {
		i32 u0x020000d5, ; uint32_t type_token_id
		i32 10; uint32_t java_map_index
	}, ; 53
	%struct.TypeMapModuleEntry {
		i32 u0x020000de, ; uint32_t type_token_id
		i32 73; uint32_t java_map_index
	}, ; 54
	%struct.TypeMapModuleEntry {
		i32 u0x020000e0, ; uint32_t type_token_id
		i32 9; uint32_t java_map_index
	}, ; 55
	%struct.TypeMapModuleEntry {
		i32 u0x020000e1, ; uint32_t type_token_id
		i32 38; uint32_t java_map_index
	}, ; 56
	%struct.TypeMapModuleEntry {
		i32 u0x020000e2, ; uint32_t type_token_id
		i32 148; uint32_t java_map_index
	}, ; 57
	%struct.TypeMapModuleEntry {
		i32 u0x020000ee, ; uint32_t type_token_id
		i32 87; uint32_t java_map_index
	}, ; 58
	%struct.TypeMapModuleEntry {
		i32 u0x020000f6, ; uint32_t type_token_id
		i32 117; uint32_t java_map_index
	}, ; 59
	%struct.TypeMapModuleEntry {
		i32 u0x020000f7, ; uint32_t type_token_id
		i32 67; uint32_t java_map_index
	}, ; 60
	%struct.TypeMapModuleEntry {
		i32 u0x020000fa, ; uint32_t type_token_id
		i32 104; uint32_t java_map_index
	}, ; 61
	%struct.TypeMapModuleEntry {
		i32 u0x020000fb, ; uint32_t type_token_id
		i32 146; uint32_t java_map_index
	}, ; 62
	%struct.TypeMapModuleEntry {
		i32 u0x020000fc, ; uint32_t type_token_id
		i32 109; uint32_t java_map_index
	}, ; 63
	%struct.TypeMapModuleEntry {
		i32 u0x020000fd, ; uint32_t type_token_id
		i32 16; uint32_t java_map_index
	}, ; 64
	%struct.TypeMapModuleEntry {
		i32 u0x020000ff, ; uint32_t type_token_id
		i32 12; uint32_t java_map_index
	}, ; 65
	%struct.TypeMapModuleEntry {
		i32 u0x02000100, ; uint32_t type_token_id
		i32 58; uint32_t java_map_index
	}, ; 66
	%struct.TypeMapModuleEntry {
		i32 u0x02000102, ; uint32_t type_token_id
		i32 71; uint32_t java_map_index
	}, ; 67
	%struct.TypeMapModuleEntry {
		i32 u0x02000103, ; uint32_t type_token_id
		i32 34; uint32_t java_map_index
	}, ; 68
	%struct.TypeMapModuleEntry {
		i32 u0x02000105, ; uint32_t type_token_id
		i32 59; uint32_t java_map_index
	}, ; 69
	%struct.TypeMapModuleEntry {
		i32 u0x02000106, ; uint32_t type_token_id
		i32 33; uint32_t java_map_index
	}, ; 70
	%struct.TypeMapModuleEntry {
		i32 u0x02000107, ; uint32_t type_token_id
		i32 86; uint32_t java_map_index
	}, ; 71
	%struct.TypeMapModuleEntry {
		i32 u0x02000108, ; uint32_t type_token_id
		i32 18; uint32_t java_map_index
	}, ; 72
	%struct.TypeMapModuleEntry {
		i32 u0x0200010b, ; uint32_t type_token_id
		i32 56; uint32_t java_map_index
	}, ; 73
	%struct.TypeMapModuleEntry {
		i32 u0x0200010c, ; uint32_t type_token_id
		i32 124; uint32_t java_map_index
	}, ; 74
	%struct.TypeMapModuleEntry {
		i32 u0x02000111, ; uint32_t type_token_id
		i32 44; uint32_t java_map_index
	}, ; 75
	%struct.TypeMapModuleEntry {
		i32 u0x02000113, ; uint32_t type_token_id
		i32 106; uint32_t java_map_index
	}, ; 76
	%struct.TypeMapModuleEntry {
		i32 u0x02000115, ; uint32_t type_token_id
		i32 4; uint32_t java_map_index
	}, ; 77
	%struct.TypeMapModuleEntry {
		i32 u0x02000116, ; uint32_t type_token_id
		i32 41; uint32_t java_map_index
	}, ; 78
	%struct.TypeMapModuleEntry {
		i32 u0x02000117, ; uint32_t type_token_id
		i32 97; uint32_t java_map_index
	}, ; 79
	%struct.TypeMapModuleEntry {
		i32 u0x02000118, ; uint32_t type_token_id
		i32 13; uint32_t java_map_index
	}, ; 80
	%struct.TypeMapModuleEntry {
		i32 u0x02000119, ; uint32_t type_token_id
		i32 101; uint32_t java_map_index
	}, ; 81
	%struct.TypeMapModuleEntry {
		i32 u0x0200011b, ; uint32_t type_token_id
		i32 95; uint32_t java_map_index
	}, ; 82
	%struct.TypeMapModuleEntry {
		i32 u0x0200011c, ; uint32_t type_token_id
		i32 64; uint32_t java_map_index
	}, ; 83
	%struct.TypeMapModuleEntry {
		i32 u0x0200011d, ; uint32_t type_token_id
		i32 144; uint32_t java_map_index
	}, ; 84
	%struct.TypeMapModuleEntry {
		i32 u0x0200011e, ; uint32_t type_token_id
		i32 78; uint32_t java_map_index
	}, ; 85
	%struct.TypeMapModuleEntry {
		i32 u0x0200011f, ; uint32_t type_token_id
		i32 40; uint32_t java_map_index
	}, ; 86
	%struct.TypeMapModuleEntry {
		i32 u0x02000121, ; uint32_t type_token_id
		i32 77; uint32_t java_map_index
	}, ; 87
	%struct.TypeMapModuleEntry {
		i32 u0x02000123, ; uint32_t type_token_id
		i32 36; uint32_t java_map_index
	}, ; 88
	%struct.TypeMapModuleEntry {
		i32 u0x02000125, ; uint32_t type_token_id
		i32 151; uint32_t java_map_index
	}, ; 89
	%struct.TypeMapModuleEntry {
		i32 u0x02000127, ; uint32_t type_token_id
		i32 68; uint32_t java_map_index
	}, ; 90
	%struct.TypeMapModuleEntry {
		i32 u0x02000128, ; uint32_t type_token_id
		i32 133; uint32_t java_map_index
	}, ; 91
	%struct.TypeMapModuleEntry {
		i32 u0x02000129, ; uint32_t type_token_id
		i32 134; uint32_t java_map_index
	}, ; 92
	%struct.TypeMapModuleEntry {
		i32 u0x0200012a, ; uint32_t type_token_id
		i32 145; uint32_t java_map_index
	}, ; 93
	%struct.TypeMapModuleEntry {
		i32 u0x0200012c, ; uint32_t type_token_id
		i32 15; uint32_t java_map_index
	}, ; 94
	%struct.TypeMapModuleEntry {
		i32 u0x0200012e, ; uint32_t type_token_id
		i32 72; uint32_t java_map_index
	}, ; 95
	%struct.TypeMapModuleEntry {
		i32 u0x0200012f, ; uint32_t type_token_id
		i32 14; uint32_t java_map_index
	}, ; 96
	%struct.TypeMapModuleEntry {
		i32 u0x02000131, ; uint32_t type_token_id
		i32 132; uint32_t java_map_index
	}, ; 97
	%struct.TypeMapModuleEntry {
		i32 u0x02000133, ; uint32_t type_token_id
		i32 138; uint32_t java_map_index
	}, ; 98
	%struct.TypeMapModuleEntry {
		i32 u0x02000135, ; uint32_t type_token_id
		i32 93; uint32_t java_map_index
	}, ; 99
	%struct.TypeMapModuleEntry {
		i32 u0x02000136, ; uint32_t type_token_id
		i32 141; uint32_t java_map_index
	}, ; 100
	%struct.TypeMapModuleEntry {
		i32 u0x02000137, ; uint32_t type_token_id
		i32 83; uint32_t java_map_index
	}, ; 101
	%struct.TypeMapModuleEntry {
		i32 u0x02000139, ; uint32_t type_token_id
		i32 21; uint32_t java_map_index
	}, ; 102
	%struct.TypeMapModuleEntry {
		i32 u0x0200013a, ; uint32_t type_token_id
		i32 63; uint32_t java_map_index
	}, ; 103
	%struct.TypeMapModuleEntry {
		i32 u0x0200013b, ; uint32_t type_token_id
		i32 61; uint32_t java_map_index
	}, ; 104
	%struct.TypeMapModuleEntry {
		i32 u0x0200013d, ; uint32_t type_token_id
		i32 100; uint32_t java_map_index
	}, ; 105
	%struct.TypeMapModuleEntry {
		i32 u0x0200013f, ; uint32_t type_token_id
		i32 142; uint32_t java_map_index
	}, ; 106
	%struct.TypeMapModuleEntry {
		i32 u0x02000140, ; uint32_t type_token_id
		i32 122; uint32_t java_map_index
	}, ; 107
	%struct.TypeMapModuleEntry {
		i32 u0x02000141, ; uint32_t type_token_id
		i32 43; uint32_t java_map_index
	}, ; 108
	%struct.TypeMapModuleEntry {
		i32 u0x02000143, ; uint32_t type_token_id
		i32 140; uint32_t java_map_index
	}, ; 109
	%struct.TypeMapModuleEntry {
		i32 u0x02000145, ; uint32_t type_token_id
		i32 69; uint32_t java_map_index
	}, ; 110
	%struct.TypeMapModuleEntry {
		i32 u0x02000147, ; uint32_t type_token_id
		i32 5; uint32_t java_map_index
	}, ; 111
	%struct.TypeMapModuleEntry {
		i32 u0x02000148, ; uint32_t type_token_id
		i32 130; uint32_t java_map_index
	}, ; 112
	%struct.TypeMapModuleEntry {
		i32 u0x0200014a, ; uint32_t type_token_id
		i32 35; uint32_t java_map_index
	}, ; 113
	%struct.TypeMapModuleEntry {
		i32 u0x0200014b, ; uint32_t type_token_id
		i32 1; uint32_t java_map_index
	}, ; 114
	%struct.TypeMapModuleEntry {
		i32 u0x0200014c, ; uint32_t type_token_id
		i32 85; uint32_t java_map_index
	}, ; 115
	%struct.TypeMapModuleEntry {
		i32 u0x0200014d, ; uint32_t type_token_id
		i32 47; uint32_t java_map_index
	}, ; 116
	%struct.TypeMapModuleEntry {
		i32 u0x0200014e, ; uint32_t type_token_id
		i32 116; uint32_t java_map_index
	}, ; 117
	%struct.TypeMapModuleEntry {
		i32 u0x0200014f, ; uint32_t type_token_id
		i32 125; uint32_t java_map_index
	}, ; 118
	%struct.TypeMapModuleEntry {
		i32 u0x02000150, ; uint32_t type_token_id
		i32 108; uint32_t java_map_index
	}, ; 119
	%struct.TypeMapModuleEntry {
		i32 u0x02000151, ; uint32_t type_token_id
		i32 91; uint32_t java_map_index
	}, ; 120
	%struct.TypeMapModuleEntry {
		i32 u0x02000152, ; uint32_t type_token_id
		i32 81; uint32_t java_map_index
	}, ; 121
	%struct.TypeMapModuleEntry {
		i32 u0x02000153, ; uint32_t type_token_id
		i32 107; uint32_t java_map_index
	}, ; 122
	%struct.TypeMapModuleEntry {
		i32 u0x02000154, ; uint32_t type_token_id
		i32 137; uint32_t java_map_index
	}, ; 123
	%struct.TypeMapModuleEntry {
		i32 u0x02000155, ; uint32_t type_token_id
		i32 115; uint32_t java_map_index
	}, ; 124
	%struct.TypeMapModuleEntry {
		i32 u0x02000156, ; uint32_t type_token_id
		i32 82; uint32_t java_map_index
	}, ; 125
	%struct.TypeMapModuleEntry {
		i32 u0x02000157, ; uint32_t type_token_id
		i32 60; uint32_t java_map_index
	}, ; 126
	%struct.TypeMapModuleEntry {
		i32 u0x02000158, ; uint32_t type_token_id
		i32 103; uint32_t java_map_index
	}, ; 127
	%struct.TypeMapModuleEntry {
		i32 u0x0200015a, ; uint32_t type_token_id
		i32 89; uint32_t java_map_index
	}, ; 128
	%struct.TypeMapModuleEntry {
		i32 u0x0200015b, ; uint32_t type_token_id
		i32 118; uint32_t java_map_index
	}, ; 129
	%struct.TypeMapModuleEntry {
		i32 u0x0200015c, ; uint32_t type_token_id
		i32 88; uint32_t java_map_index
	}, ; 130
	%struct.TypeMapModuleEntry {
		i32 u0x0200015d, ; uint32_t type_token_id
		i32 48; uint32_t java_map_index
	}, ; 131
	%struct.TypeMapModuleEntry {
		i32 u0x0200015e, ; uint32_t type_token_id
		i32 119; uint32_t java_map_index
	}, ; 132
	%struct.TypeMapModuleEntry {
		i32 u0x02000160, ; uint32_t type_token_id
		i32 24; uint32_t java_map_index
	}, ; 133
	%struct.TypeMapModuleEntry {
		i32 u0x02000163, ; uint32_t type_token_id
		i32 105; uint32_t java_map_index
	}, ; 134
	%struct.TypeMapModuleEntry {
		i32 u0x02000164, ; uint32_t type_token_id
		i32 37; uint32_t java_map_index
	}, ; 135
	%struct.TypeMapModuleEntry {
		i32 u0x02000165, ; uint32_t type_token_id
		i32 143; uint32_t java_map_index
	}, ; 136
	%struct.TypeMapModuleEntry {
		i32 u0x02000166, ; uint32_t type_token_id
		i32 149; uint32_t java_map_index
	}, ; 137
	%struct.TypeMapModuleEntry {
		i32 u0x02000168, ; uint32_t type_token_id
		i32 3; uint32_t java_map_index
	}, ; 138
	%struct.TypeMapModuleEntry {
		i32 u0x02000169, ; uint32_t type_token_id
		i32 51; uint32_t java_map_index
	}, ; 139
	%struct.TypeMapModuleEntry {
		i32 u0x0200016a, ; uint32_t type_token_id
		i32 131; uint32_t java_map_index
	}, ; 140
	%struct.TypeMapModuleEntry {
		i32 u0x0200016b, ; uint32_t type_token_id
		i32 96; uint32_t java_map_index
	}, ; 141
	%struct.TypeMapModuleEntry {
		i32 u0x0200016c, ; uint32_t type_token_id
		i32 50; uint32_t java_map_index
	}, ; 142
	%struct.TypeMapModuleEntry {
		i32 u0x0200016e, ; uint32_t type_token_id
		i32 90; uint32_t java_map_index
	}, ; 143
	%struct.TypeMapModuleEntry {
		i32 u0x0200016f, ; uint32_t type_token_id
		i32 32; uint32_t java_map_index
	}, ; 144
	%struct.TypeMapModuleEntry {
		i32 u0x02000170, ; uint32_t type_token_id
		i32 65; uint32_t java_map_index
	}, ; 145
	%struct.TypeMapModuleEntry {
		i32 u0x02000171, ; uint32_t type_token_id
		i32 128; uint32_t java_map_index
	}, ; 146
	%struct.TypeMapModuleEntry {
		i32 u0x02000180, ; uint32_t type_token_id
		i32 70; uint32_t java_map_index
	} ; 147
], align 4

@module1_managed_to_java_duplicates = internal dso_local constant [51 x %struct.TypeMapModuleEntry] [
	%struct.TypeMapModuleEntry {
		i32 u0x02000060, ; uint32_t type_token_id
		i32 147; uint32_t java_map_index
	}, ; 0
	%struct.TypeMapModuleEntry {
		i32 u0x02000062, ; uint32_t type_token_id
		i32 29; uint32_t java_map_index
	}, ; 1
	%struct.TypeMapModuleEntry {
		i32 u0x02000065, ; uint32_t type_token_id
		i32 23; uint32_t java_map_index
	}, ; 2
	%struct.TypeMapModuleEntry {
		i32 u0x02000067, ; uint32_t type_token_id
		i32 49; uint32_t java_map_index
	}, ; 3
	%struct.TypeMapModuleEntry {
		i32 u0x02000069, ; uint32_t type_token_id
		i32 139; uint32_t java_map_index
	}, ; 4
	%struct.TypeMapModuleEntry {
		i32 u0x0200006b, ; uint32_t type_token_id
		i32 84; uint32_t java_map_index
	}, ; 5
	%struct.TypeMapModuleEntry {
		i32 u0x0200006d, ; uint32_t type_token_id
		i32 99; uint32_t java_map_index
	}, ; 6
	%struct.TypeMapModuleEntry {
		i32 u0x0200006f, ; uint32_t type_token_id
		i32 129; uint32_t java_map_index
	}, ; 7
	%struct.TypeMapModuleEntry {
		i32 u0x02000071, ; uint32_t type_token_id
		i32 113; uint32_t java_map_index
	}, ; 8
	%struct.TypeMapModuleEntry {
		i32 u0x02000075, ; uint32_t type_token_id
		i32 102; uint32_t java_map_index
	}, ; 9
	%struct.TypeMapModuleEntry {
		i32 u0x02000078, ; uint32_t type_token_id
		i32 42; uint32_t java_map_index
	}, ; 10
	%struct.TypeMapModuleEntry {
		i32 u0x0200007b, ; uint32_t type_token_id
		i32 110; uint32_t java_map_index
	}, ; 11
	%struct.TypeMapModuleEntry {
		i32 u0x0200007e, ; uint32_t type_token_id
		i32 150; uint32_t java_map_index
	}, ; 12
	%struct.TypeMapModuleEntry {
		i32 u0x02000082, ; uint32_t type_token_id
		i32 135; uint32_t java_map_index
	}, ; 13
	%struct.TypeMapModuleEntry {
		i32 u0x0200008a, ; uint32_t type_token_id
		i32 30; uint32_t java_map_index
	}, ; 14
	%struct.TypeMapModuleEntry {
		i32 u0x0200008d, ; uint32_t type_token_id
		i32 30; uint32_t java_map_index
	}, ; 15
	%struct.TypeMapModuleEntry {
		i32 u0x0200008e, ; uint32_t type_token_id
		i32 136; uint32_t java_map_index
	}, ; 16
	%struct.TypeMapModuleEntry {
		i32 u0x02000090, ; uint32_t type_token_id
		i32 62; uint32_t java_map_index
	}, ; 17
	%struct.TypeMapModuleEntry {
		i32 u0x02000094, ; uint32_t type_token_id
		i32 6; uint32_t java_map_index
	}, ; 18
	%struct.TypeMapModuleEntry {
		i32 u0x02000096, ; uint32_t type_token_id
		i32 11; uint32_t java_map_index
	}, ; 19
	%struct.TypeMapModuleEntry {
		i32 u0x0200009f, ; uint32_t type_token_id
		i32 20; uint32_t java_map_index
	}, ; 20
	%struct.TypeMapModuleEntry {
		i32 u0x020000ac, ; uint32_t type_token_id
		i32 45; uint32_t java_map_index
	}, ; 21
	%struct.TypeMapModuleEntry {
		i32 u0x020000b7, ; uint32_t type_token_id
		i32 25; uint32_t java_map_index
	}, ; 22
	%struct.TypeMapModuleEntry {
		i32 u0x020000bc, ; uint32_t type_token_id
		i32 39; uint32_t java_map_index
	}, ; 23
	%struct.TypeMapModuleEntry {
		i32 u0x020000d4, ; uint32_t type_token_id
		i32 123; uint32_t java_map_index
	}, ; 24
	%struct.TypeMapModuleEntry {
		i32 u0x020000da, ; uint32_t type_token_id
		i32 10; uint32_t java_map_index
	}, ; 25
	%struct.TypeMapModuleEntry {
		i32 u0x020000df, ; uint32_t type_token_id
		i32 73; uint32_t java_map_index
	}, ; 26
	%struct.TypeMapModuleEntry {
		i32 u0x020000e3, ; uint32_t type_token_id
		i32 148; uint32_t java_map_index
	}, ; 27
	%struct.TypeMapModuleEntry {
		i32 u0x02000104, ; uint32_t type_token_id
		i32 71; uint32_t java_map_index
	}, ; 28
	%struct.TypeMapModuleEntry {
		i32 u0x02000114, ; uint32_t type_token_id
		i32 106; uint32_t java_map_index
	}, ; 29
	%struct.TypeMapModuleEntry {
		i32 u0x0200011a, ; uint32_t type_token_id
		i32 101; uint32_t java_map_index
	}, ; 30
	%struct.TypeMapModuleEntry {
		i32 u0x02000120, ; uint32_t type_token_id
		i32 40; uint32_t java_map_index
	}, ; 31
	%struct.TypeMapModuleEntry {
		i32 u0x02000122, ; uint32_t type_token_id
		i32 77; uint32_t java_map_index
	}, ; 32
	%struct.TypeMapModuleEntry {
		i32 u0x02000124, ; uint32_t type_token_id
		i32 36; uint32_t java_map_index
	}, ; 33
	%struct.TypeMapModuleEntry {
		i32 u0x02000126, ; uint32_t type_token_id
		i32 151; uint32_t java_map_index
	}, ; 34
	%struct.TypeMapModuleEntry {
		i32 u0x0200012b, ; uint32_t type_token_id
		i32 145; uint32_t java_map_index
	}, ; 35
	%struct.TypeMapModuleEntry {
		i32 u0x0200012d, ; uint32_t type_token_id
		i32 15; uint32_t java_map_index
	}, ; 36
	%struct.TypeMapModuleEntry {
		i32 u0x02000130, ; uint32_t type_token_id
		i32 14; uint32_t java_map_index
	}, ; 37
	%struct.TypeMapModuleEntry {
		i32 u0x02000132, ; uint32_t type_token_id
		i32 132; uint32_t java_map_index
	}, ; 38
	%struct.TypeMapModuleEntry {
		i32 u0x02000134, ; uint32_t type_token_id
		i32 138; uint32_t java_map_index
	}, ; 39
	%struct.TypeMapModuleEntry {
		i32 u0x02000138, ; uint32_t type_token_id
		i32 83; uint32_t java_map_index
	}, ; 40
	%struct.TypeMapModuleEntry {
		i32 u0x0200013c, ; uint32_t type_token_id
		i32 61; uint32_t java_map_index
	}, ; 41
	%struct.TypeMapModuleEntry {
		i32 u0x0200013e, ; uint32_t type_token_id
		i32 100; uint32_t java_map_index
	}, ; 42
	%struct.TypeMapModuleEntry {
		i32 u0x02000142, ; uint32_t type_token_id
		i32 43; uint32_t java_map_index
	}, ; 43
	%struct.TypeMapModuleEntry {
		i32 u0x02000144, ; uint32_t type_token_id
		i32 140; uint32_t java_map_index
	}, ; 44
	%struct.TypeMapModuleEntry {
		i32 u0x02000146, ; uint32_t type_token_id
		i32 69; uint32_t java_map_index
	}, ; 45
	%struct.TypeMapModuleEntry {
		i32 u0x02000149, ; uint32_t type_token_id
		i32 130; uint32_t java_map_index
	}, ; 46
	%struct.TypeMapModuleEntry {
		i32 u0x0200015f, ; uint32_t type_token_id
		i32 119; uint32_t java_map_index
	}, ; 47
	%struct.TypeMapModuleEntry {
		i32 u0x02000161, ; uint32_t type_token_id
		i32 81; uint32_t java_map_index
	}, ; 48
	%struct.TypeMapModuleEntry {
		i32 u0x02000167, ; uint32_t type_token_id
		i32 149; uint32_t java_map_index
	}, ; 49
	%struct.TypeMapModuleEntry {
		i32 u0x0200016d, ; uint32_t type_token_id
		i32 50; uint32_t java_map_index
	} ; 50
], align 4

; Java to managed map
@map_java = dso_local local_unnamed_addr constant [152 x %struct.TypeMapJava] [
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000072, ; uint32_t type_token_id
		i32 10; uint32_t java_name_index
	}, ; 0
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200014b, ; uint32_t type_token_id
		i32 114; uint32_t java_name_index
	}, ; 1
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000073, ; uint32_t type_token_id
		i32 11; uint32_t java_name_index
	}, ; 2
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000168, ; uint32_t type_token_id
		i32 138; uint32_t java_name_index
	}, ; 3
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000115, ; uint32_t type_token_id
		i32 77; uint32_t java_name_index
	}, ; 4
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000147, ; uint32_t type_token_id
		i32 111; uint32_t java_name_index
	}, ; 5
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 26; uint32_t java_name_index
	}, ; 6
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000a3, ; uint32_t type_token_id
		i32 37; uint32_t java_name_index
	}, ; 7
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000092, ; uint32_t type_token_id
		i32 25; uint32_t java_name_index
	}, ; 8
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000e0, ; uint32_t type_token_id
		i32 55; uint32_t java_name_index
	}, ; 9
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000d5, ; uint32_t type_token_id
		i32 53; uint32_t java_name_index
	}, ; 10
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 27; uint32_t java_name_index
	}, ; 11
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000ff, ; uint32_t type_token_id
		i32 65; uint32_t java_name_index
	}, ; 12
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000118, ; uint32_t type_token_id
		i32 80; uint32_t java_name_index
	}, ; 13
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200012f, ; uint32_t type_token_id
		i32 96; uint32_t java_name_index
	}, ; 14
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 94; uint32_t java_name_index
	}, ; 15
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000fd, ; uint32_t type_token_id
		i32 64; uint32_t java_name_index
	}, ; 16
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000076, ; uint32_t type_token_id
		i32 13; uint32_t java_name_index
	}, ; 17
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000108, ; uint32_t type_token_id
		i32 72; uint32_t java_name_index
	}, ; 18
	%struct.TypeMapJava {
		i32 0, ; uint32_t module_index
		i32 u0x02000223, ; uint32_t type_token_id
		i32 150; uint32_t java_name_index
	}, ; 19
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 33; uint32_t java_name_index
	}, ; 20
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000139, ; uint32_t type_token_id
		i32 102; uint32_t java_name_index
	}, ; 21
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000a2, ; uint32_t type_token_id
		i32 36; uint32_t java_name_index
	}, ; 22
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000064, ; uint32_t type_token_id
		i32 3; uint32_t java_name_index
	}, ; 23
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000160, ; uint32_t type_token_id
		i32 133; uint32_t java_name_index
	}, ; 24
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000b6, ; uint32_t type_token_id
		i32 49; uint32_t java_name_index
	}, ; 25
	%struct.TypeMapJava {
		i32 0, ; uint32_t module_index
		i32 u0x02000224, ; uint32_t type_token_id
		i32 151; uint32_t java_name_index
	}, ; 26
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000097, ; uint32_t type_token_id
		i32 28; uint32_t java_name_index
	}, ; 27
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000091, ; uint32_t type_token_id
		i32 24; uint32_t java_name_index
	}, ; 28
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000061, ; uint32_t type_token_id
		i32 1; uint32_t java_name_index
	}, ; 29
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200007c, ; uint32_t type_token_id
		i32 16; uint32_t java_name_index
	}, ; 30
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000a1, ; uint32_t type_token_id
		i32 35; uint32_t java_name_index
	}, ; 31
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200016f, ; uint32_t type_token_id
		i32 144; uint32_t java_name_index
	}, ; 32
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000106, ; uint32_t type_token_id
		i32 70; uint32_t java_name_index
	}, ; 33
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000103, ; uint32_t type_token_id
		i32 68; uint32_t java_name_index
	}, ; 34
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200014a, ; uint32_t type_token_id
		i32 113; uint32_t java_name_index
	}, ; 35
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 88; uint32_t java_name_index
	}, ; 36
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000164, ; uint32_t type_token_id
		i32 135; uint32_t java_name_index
	}, ; 37
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000e1, ; uint32_t type_token_id
		i32 56; uint32_t java_name_index
	}, ; 38
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000bb, ; uint32_t type_token_id
		i32 50; uint32_t java_name_index
	}, ; 39
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200011f, ; uint32_t type_token_id
		i32 86; uint32_t java_name_index
	}, ; 40
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000116, ; uint32_t type_token_id
		i32 78; uint32_t java_name_index
	}, ; 41
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000077, ; uint32_t type_token_id
		i32 14; uint32_t java_name_index
	}, ; 42
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000141, ; uint32_t type_token_id
		i32 108; uint32_t java_name_index
	}, ; 43
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000111, ; uint32_t type_token_id
		i32 75; uint32_t java_name_index
	}, ; 44
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 44; uint32_t java_name_index
	}, ; 45
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000098, ; uint32_t type_token_id
		i32 29; uint32_t java_name_index
	}, ; 46
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200014d, ; uint32_t type_token_id
		i32 116; uint32_t java_name_index
	}, ; 47
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200015d, ; uint32_t type_token_id
		i32 131; uint32_t java_name_index
	}, ; 48
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000066, ; uint32_t type_token_id
		i32 4; uint32_t java_name_index
	}, ; 49
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200016c, ; uint32_t type_token_id
		i32 142; uint32_t java_name_index
	}, ; 50
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000169, ; uint32_t type_token_id
		i32 139; uint32_t java_name_index
	}, ; 51
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000a5, ; uint32_t type_token_id
		i32 39; uint32_t java_name_index
	}, ; 52
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000a7, ; uint32_t type_token_id
		i32 40; uint32_t java_name_index
	}, ; 53
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000a8, ; uint32_t type_token_id
		i32 41; uint32_t java_name_index
	}, ; 54
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200008c, ; uint32_t type_token_id
		i32 22; uint32_t java_name_index
	}, ; 55
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200010b, ; uint32_t type_token_id
		i32 73; uint32_t java_name_index
	}, ; 56
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000b2, ; uint32_t type_token_id
		i32 46; uint32_t java_name_index
	}, ; 57
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000100, ; uint32_t type_token_id
		i32 66; uint32_t java_name_index
	}, ; 58
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000105, ; uint32_t type_token_id
		i32 69; uint32_t java_name_index
	}, ; 59
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000157, ; uint32_t type_token_id
		i32 126; uint32_t java_name_index
	}, ; 60
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 104; uint32_t java_name_index
	}, ; 61
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200008f, ; uint32_t type_token_id
		i32 23; uint32_t java_name_index
	}, ; 62
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200013a, ; uint32_t type_token_id
		i32 103; uint32_t java_name_index
	}, ; 63
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200011c, ; uint32_t type_token_id
		i32 83; uint32_t java_name_index
	}, ; 64
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000170, ; uint32_t type_token_id
		i32 145; uint32_t java_name_index
	}, ; 65
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000b3, ; uint32_t type_token_id
		i32 47; uint32_t java_name_index
	}, ; 66
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000f7, ; uint32_t type_token_id
		i32 60; uint32_t java_name_index
	}, ; 67
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000127, ; uint32_t type_token_id
		i32 90; uint32_t java_name_index
	}, ; 68
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 110; uint32_t java_name_index
	}, ; 69
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000180, ; uint32_t type_token_id
		i32 147; uint32_t java_name_index
	}, ; 70
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000102, ; uint32_t type_token_id
		i32 67; uint32_t java_name_index
	}, ; 71
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200012e, ; uint32_t type_token_id
		i32 95; uint32_t java_name_index
	}, ; 72
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000de, ; uint32_t type_token_id
		i32 54; uint32_t java_name_index
	}, ; 73
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000063, ; uint32_t type_token_id
		i32 2; uint32_t java_name_index
	}, ; 74
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000084, ; uint32_t type_token_id
		i32 20; uint32_t java_name_index
	}, ; 75
	%struct.TypeMapJava {
		i32 0, ; uint32_t module_index
		i32 u0x02000221, ; uint32_t type_token_id
		i32 149; uint32_t java_name_index
	}, ; 76
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 87; uint32_t java_name_index
	}, ; 77
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200011e, ; uint32_t type_token_id
		i32 85; uint32_t java_name_index
	}, ; 78
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000099, ; uint32_t type_token_id
		i32 30; uint32_t java_name_index
	}, ; 79
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000a0, ; uint32_t type_token_id
		i32 34; uint32_t java_name_index
	}, ; 80
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 121; uint32_t java_name_index
	}, ; 81
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000156, ; uint32_t type_token_id
		i32 125; uint32_t java_name_index
	}, ; 82
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000137, ; uint32_t type_token_id
		i32 101; uint32_t java_name_index
	}, ; 83
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 6; uint32_t java_name_index
	}, ; 84
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200014c, ; uint32_t type_token_id
		i32 115; uint32_t java_name_index
	}, ; 85
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000107, ; uint32_t type_token_id
		i32 71; uint32_t java_name_index
	}, ; 86
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000ee, ; uint32_t type_token_id
		i32 58; uint32_t java_name_index
	}, ; 87
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200015c, ; uint32_t type_token_id
		i32 130; uint32_t java_name_index
	}, ; 88
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200015a, ; uint32_t type_token_id
		i32 128; uint32_t java_name_index
	}, ; 89
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200016e, ; uint32_t type_token_id
		i32 143; uint32_t java_name_index
	}, ; 90
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000151, ; uint32_t type_token_id
		i32 120; uint32_t java_name_index
	}, ; 91
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000d1, ; uint32_t type_token_id
		i32 51; uint32_t java_name_index
	}, ; 92
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000135, ; uint32_t type_token_id
		i32 99; uint32_t java_name_index
	}, ; 93
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000b4, ; uint32_t type_token_id
		i32 48; uint32_t java_name_index
	}, ; 94
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200011b, ; uint32_t type_token_id
		i32 82; uint32_t java_name_index
	}, ; 95
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200016b, ; uint32_t type_token_id
		i32 141; uint32_t java_name_index
	}, ; 96
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000117, ; uint32_t type_token_id
		i32 79; uint32_t java_name_index
	}, ; 97
	%struct.TypeMapJava {
		i32 0, ; uint32_t module_index
		i32 u0x02000219, ; uint32_t type_token_id
		i32 148; uint32_t java_name_index
	}, ; 98
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 7; uint32_t java_name_index
	}, ; 99
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200013d, ; uint32_t type_token_id
		i32 105; uint32_t java_name_index
	}, ; 100
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000119, ; uint32_t type_token_id
		i32 81; uint32_t java_name_index
	}, ; 101
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000074, ; uint32_t type_token_id
		i32 12; uint32_t java_name_index
	}, ; 102
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000158, ; uint32_t type_token_id
		i32 127; uint32_t java_name_index
	}, ; 103
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000fa, ; uint32_t type_token_id
		i32 61; uint32_t java_name_index
	}, ; 104
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000163, ; uint32_t type_token_id
		i32 134; uint32_t java_name_index
	}, ; 105
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000113, ; uint32_t type_token_id
		i32 76; uint32_t java_name_index
	}, ; 106
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000153, ; uint32_t type_token_id
		i32 122; uint32_t java_name_index
	}, ; 107
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000150, ; uint32_t type_token_id
		i32 119; uint32_t java_name_index
	}, ; 108
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000fc, ; uint32_t type_token_id
		i32 63; uint32_t java_name_index
	}, ; 109
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000079, ; uint32_t type_token_id
		i32 15; uint32_t java_name_index
	}, ; 110
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000ae, ; uint32_t type_token_id
		i32 45; uint32_t java_name_index
	}, ; 111
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000aa, ; uint32_t type_token_id
		i32 43; uint32_t java_name_index
	}, ; 112
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 9; uint32_t java_name_index
	}, ; 113
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000a9, ; uint32_t type_token_id
		i32 42; uint32_t java_name_index
	}, ; 114
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000155, ; uint32_t type_token_id
		i32 124; uint32_t java_name_index
	}, ; 115
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200014e, ; uint32_t type_token_id
		i32 117; uint32_t java_name_index
	}, ; 116
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000f6, ; uint32_t type_token_id
		i32 59; uint32_t java_name_index
	}, ; 117
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200015b, ; uint32_t type_token_id
		i32 129; uint32_t java_name_index
	}, ; 118
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200015e, ; uint32_t type_token_id
		i32 132; uint32_t java_name_index
	}, ; 119
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000080, ; uint32_t type_token_id
		i32 18; uint32_t java_name_index
	}, ; 120
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200009b, ; uint32_t type_token_id
		i32 31; uint32_t java_name_index
	}, ; 121
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000140, ; uint32_t type_token_id
		i32 107; uint32_t java_name_index
	}, ; 122
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000d3, ; uint32_t type_token_id
		i32 52; uint32_t java_name_index
	}, ; 123
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200010c, ; uint32_t type_token_id
		i32 74; uint32_t java_name_index
	}, ; 124
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200014f, ; uint32_t type_token_id
		i32 118; uint32_t java_name_index
	}, ; 125
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000a4, ; uint32_t type_token_id
		i32 38; uint32_t java_name_index
	}, ; 126
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200009d, ; uint32_t type_token_id
		i32 32; uint32_t java_name_index
	}, ; 127
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000171, ; uint32_t type_token_id
		i32 146; uint32_t java_name_index
	}, ; 128
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 8; uint32_t java_name_index
	}, ; 129
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 112; uint32_t java_name_index
	}, ; 130
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200016a, ; uint32_t type_token_id
		i32 140; uint32_t java_name_index
	}, ; 131
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000131, ; uint32_t type_token_id
		i32 97; uint32_t java_name_index
	}, ; 132
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000128, ; uint32_t type_token_id
		i32 91; uint32_t java_name_index
	}, ; 133
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000129, ; uint32_t type_token_id
		i32 92; uint32_t java_name_index
	}, ; 134
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 19; uint32_t java_name_index
	}, ; 135
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 21; uint32_t java_name_index
	}, ; 136
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000154, ; uint32_t type_token_id
		i32 123; uint32_t java_name_index
	}, ; 137
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000133, ; uint32_t type_token_id
		i32 98; uint32_t java_name_index
	}, ; 138
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 5; uint32_t java_name_index
	}, ; 139
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 109; uint32_t java_name_index
	}, ; 140
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000136, ; uint32_t type_token_id
		i32 100; uint32_t java_name_index
	}, ; 141
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200013f, ; uint32_t type_token_id
		i32 106; uint32_t java_name_index
	}, ; 142
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x02000165, ; uint32_t type_token_id
		i32 136; uint32_t java_name_index
	}, ; 143
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200011d, ; uint32_t type_token_id
		i32 84; uint32_t java_name_index
	}, ; 144
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200012a, ; uint32_t type_token_id
		i32 93; uint32_t java_name_index
	}, ; 145
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000fb, ; uint32_t type_token_id
		i32 62; uint32_t java_name_index
	}, ; 146
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x0200005f, ; uint32_t type_token_id
		i32 0; uint32_t java_name_index
	}, ; 147
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x020000e2, ; uint32_t type_token_id
		i32 57; uint32_t java_name_index
	}, ; 148
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 137; uint32_t java_name_index
	}, ; 149
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 17; uint32_t java_name_index
	}, ; 150
	%struct.TypeMapJava {
		i32 1, ; uint32_t module_index
		i32 u0x00000000, ; uint32_t type_token_id
		i32 89; uint32_t java_name_index
	} ; 151
], align 4

; Java type names
@java_type_names = dso_local local_unnamed_addr constant [152 x ptr] [
	ptr @.str.0, ; 0
	ptr @.str.1, ; 1
	ptr @.str.2, ; 2
	ptr @.str.3, ; 3
	ptr @.str.4, ; 4
	ptr @.str.5, ; 5
	ptr @.str.6, ; 6
	ptr @.str.7, ; 7
	ptr @.str.8, ; 8
	ptr @.str.9, ; 9
	ptr @.str.10, ; 10
	ptr @.str.11, ; 11
	ptr @.str.12, ; 12
	ptr @.str.13, ; 13
	ptr @.str.14, ; 14
	ptr @.str.15, ; 15
	ptr @.str.16, ; 16
	ptr @.str.17, ; 17
	ptr @.str.18, ; 18
	ptr @.str.19, ; 19
	ptr @.str.20, ; 20
	ptr @.str.21, ; 21
	ptr @.str.22, ; 22
	ptr @.str.23, ; 23
	ptr @.str.24, ; 24
	ptr @.str.25, ; 25
	ptr @.str.26, ; 26
	ptr @.str.27, ; 27
	ptr @.str.28, ; 28
	ptr @.str.29, ; 29
	ptr @.str.30, ; 30
	ptr @.str.31, ; 31
	ptr @.str.32, ; 32
	ptr @.str.33, ; 33
	ptr @.str.34, ; 34
	ptr @.str.35, ; 35
	ptr @.str.36, ; 36
	ptr @.str.37, ; 37
	ptr @.str.38, ; 38
	ptr @.str.39, ; 39
	ptr @.str.40, ; 40
	ptr @.str.41, ; 41
	ptr @.str.42, ; 42
	ptr @.str.43, ; 43
	ptr @.str.44, ; 44
	ptr @.str.45, ; 45
	ptr @.str.46, ; 46
	ptr @.str.47, ; 47
	ptr @.str.48, ; 48
	ptr @.str.49, ; 49
	ptr @.str.50, ; 50
	ptr @.str.51, ; 51
	ptr @.str.52, ; 52
	ptr @.str.53, ; 53
	ptr @.str.54, ; 54
	ptr @.str.55, ; 55
	ptr @.str.56, ; 56
	ptr @.str.57, ; 57
	ptr @.str.58, ; 58
	ptr @.str.59, ; 59
	ptr @.str.60, ; 60
	ptr @.str.61, ; 61
	ptr @.str.62, ; 62
	ptr @.str.63, ; 63
	ptr @.str.64, ; 64
	ptr @.str.65, ; 65
	ptr @.str.66, ; 66
	ptr @.str.67, ; 67
	ptr @.str.68, ; 68
	ptr @.str.69, ; 69
	ptr @.str.70, ; 70
	ptr @.str.71, ; 71
	ptr @.str.72, ; 72
	ptr @.str.73, ; 73
	ptr @.str.74, ; 74
	ptr @.str.75, ; 75
	ptr @.str.76, ; 76
	ptr @.str.77, ; 77
	ptr @.str.78, ; 78
	ptr @.str.79, ; 79
	ptr @.str.80, ; 80
	ptr @.str.81, ; 81
	ptr @.str.82, ; 82
	ptr @.str.83, ; 83
	ptr @.str.84, ; 84
	ptr @.str.85, ; 85
	ptr @.str.86, ; 86
	ptr @.str.87, ; 87
	ptr @.str.88, ; 88
	ptr @.str.89, ; 89
	ptr @.str.90, ; 90
	ptr @.str.91, ; 91
	ptr @.str.92, ; 92
	ptr @.str.93, ; 93
	ptr @.str.94, ; 94
	ptr @.str.95, ; 95
	ptr @.str.96, ; 96
	ptr @.str.97, ; 97
	ptr @.str.98, ; 98
	ptr @.str.99, ; 99
	ptr @.str.100, ; 100
	ptr @.str.101, ; 101
	ptr @.str.102, ; 102
	ptr @.str.103, ; 103
	ptr @.str.104, ; 104
	ptr @.str.105, ; 105
	ptr @.str.106, ; 106
	ptr @.str.107, ; 107
	ptr @.str.108, ; 108
	ptr @.str.109, ; 109
	ptr @.str.110, ; 110
	ptr @.str.111, ; 111
	ptr @.str.112, ; 112
	ptr @.str.113, ; 113
	ptr @.str.114, ; 114
	ptr @.str.115, ; 115
	ptr @.str.116, ; 116
	ptr @.str.117, ; 117
	ptr @.str.118, ; 118
	ptr @.str.119, ; 119
	ptr @.str.120, ; 120
	ptr @.str.121, ; 121
	ptr @.str.122, ; 122
	ptr @.str.123, ; 123
	ptr @.str.124, ; 124
	ptr @.str.125, ; 125
	ptr @.str.126, ; 126
	ptr @.str.127, ; 127
	ptr @.str.128, ; 128
	ptr @.str.129, ; 129
	ptr @.str.130, ; 130
	ptr @.str.131, ; 131
	ptr @.str.132, ; 132
	ptr @.str.133, ; 133
	ptr @.str.134, ; 134
	ptr @.str.135, ; 135
	ptr @.str.136, ; 136
	ptr @.str.137, ; 137
	ptr @.str.138, ; 138
	ptr @.str.139, ; 139
	ptr @.str.140, ; 140
	ptr @.str.141, ; 141
	ptr @.str.142, ; 142
	ptr @.str.143, ; 143
	ptr @.str.144, ; 144
	ptr @.str.145, ; 145
	ptr @.str.146, ; 146
	ptr @.str.147, ; 147
	ptr @.str.148, ; 148
	ptr @.str.149, ; 149
	ptr @.str.150, ; 150
	ptr @.str.151 ; 151
], align 8

; Strings
@.str.0 = private unnamed_addr constant [32 x i8] c"javax/security/cert/Certificate\00", align 1
@.str.1 = private unnamed_addr constant [36 x i8] c"javax/security/cert/X509Certificate\00", align 1
@.str.2 = private unnamed_addr constant [28 x i8] c"javax/security/auth/Subject\00", align 1
@.str.3 = private unnamed_addr constant [24 x i8] c"javax/net/SocketFactory\00", align 1
@.str.4 = private unnamed_addr constant [33 x i8] c"javax/net/ssl/HttpsURLConnection\00", align 1
@.str.5 = private unnamed_addr constant [31 x i8] c"javax/net/ssl/HostnameVerifier\00", align 1
@.str.6 = private unnamed_addr constant [25 x i8] c"javax/net/ssl/KeyManager\00", align 1
@.str.7 = private unnamed_addr constant [25 x i8] c"javax/net/ssl/SSLSession\00", align 1
@.str.8 = private unnamed_addr constant [32 x i8] c"javax/net/ssl/SSLSessionContext\00", align 1
@.str.9 = private unnamed_addr constant [27 x i8] c"javax/net/ssl/TrustManager\00", align 1
@.str.10 = private unnamed_addr constant [32 x i8] c"javax/net/ssl/KeyManagerFactory\00", align 1
@.str.11 = private unnamed_addr constant [25 x i8] c"javax/net/ssl/SSLContext\00", align 1
@.str.12 = private unnamed_addr constant [31 x i8] c"javax/net/ssl/SSLSocketFactory\00", align 1
@.str.13 = private unnamed_addr constant [34 x i8] c"javax/net/ssl/TrustManagerFactory\00", align 1
@.str.14 = private unnamed_addr constant [33 x i8] c"android/database/DataSetObserver\00", align 1
@.str.15 = private unnamed_addr constant [27 x i8] c"android/widget/AbsListView\00", align 1
@.str.16 = private unnamed_addr constant [27 x i8] c"android/widget/AdapterView\00", align 1
@.str.17 = private unnamed_addr constant [47 x i8] c"android/widget/AdapterView$OnItemClickListener\00", align 1
@.str.18 = private unnamed_addr constant [63 x i8] c"mono/android/widget/AdapterView_OnItemClickListenerImplementor\00", align 1
@.str.19 = private unnamed_addr constant [51 x i8] c"android/widget/AdapterView$OnItemLongClickListener\00", align 1
@.str.20 = private unnamed_addr constant [67 x i8] c"mono/android/widget/AdapterView_OnItemLongClickListenerImplementor\00", align 1
@.str.21 = private unnamed_addr constant [28 x i8] c"android/widget/ArrayAdapter\00", align 1
@.str.22 = private unnamed_addr constant [24 x i8] c"android/widget/TextView\00", align 1
@.str.23 = private unnamed_addr constant [27 x i8] c"android/widget/BaseAdapter\00", align 1
@.str.24 = private unnamed_addr constant [24 x i8] c"android/widget/EditText\00", align 1
@.str.25 = private unnamed_addr constant [27 x i8] c"android/widget/FrameLayout\00", align 1
@.str.26 = private unnamed_addr constant [23 x i8] c"android/widget/Adapter\00", align 1
@.str.27 = private unnamed_addr constant [27 x i8] c"android/widget/ListAdapter\00", align 1
@.str.28 = private unnamed_addr constant [24 x i8] c"android/widget/ListView\00", align 1
@.str.29 = private unnamed_addr constant [26 x i8] c"android/widget/ScrollView\00", align 1
@.str.30 = private unnamed_addr constant [21 x i8] c"android/widget/Toast\00", align 1
@.str.31 = private unnamed_addr constant [17 x i8] c"android/util/Log\00", align 1
@.str.32 = private unnamed_addr constant [28 x i8] c"android/util/DisplayMetrics\00", align 1
@.str.33 = private unnamed_addr constant [26 x i8] c"android/util/AttributeSet\00", align 1
@.str.34 = private unnamed_addr constant [24 x i8] c"android/util/TypedValue\00", align 1
@.str.35 = private unnamed_addr constant [23 x i8] c"android/text/TextPaint\00", align 1
@.str.36 = private unnamed_addr constant [19 x i8] c"android/os/Handler\00", align 1
@.str.37 = private unnamed_addr constant [22 x i8] c"android/os/BaseBundle\00", align 1
@.str.38 = private unnamed_addr constant [17 x i8] c"android/os/Build\00", align 1
@.str.39 = private unnamed_addr constant [25 x i8] c"android/os/Build$VERSION\00", align 1
@.str.40 = private unnamed_addr constant [18 x i8] c"android/os/Bundle\00", align 1
@.str.41 = private unnamed_addr constant [23 x i8] c"android/os/Environment\00", align 1
@.str.42 = private unnamed_addr constant [18 x i8] c"android/os/Looper\00", align 1
@.str.43 = private unnamed_addr constant [18 x i8] c"android/view/View\00", align 1
@.str.44 = private unnamed_addr constant [32 x i8] c"android/view/View$OnKeyListener\00", align 1
@.str.45 = private unnamed_addr constant [48 x i8] c"mono/android/view/View_OnKeyListenerImplementor\00", align 1
@.str.46 = private unnamed_addr constant [22 x i8] c"android/view/KeyEvent\00", align 1
@.str.47 = private unnamed_addr constant [25 x i8] c"android/view/MotionEvent\00", align 1
@.str.48 = private unnamed_addr constant [33 x i8] c"android/view/ContextThemeWrapper\00", align 1
@.str.49 = private unnamed_addr constant [24 x i8] c"android/view/InputEvent\00", align 1
@.str.50 = private unnamed_addr constant [23 x i8] c"android/view/ViewGroup\00", align 1
@.str.51 = private unnamed_addr constant [40 x i8] c"mono/android/runtime/InputStreamAdapter\00", align 1
@.str.52 = private unnamed_addr constant [21 x i8] c"java/util/Collection\00", align 1
@.str.53 = private unnamed_addr constant [18 x i8] c"java/util/HashMap\00", align 1
@.str.54 = private unnamed_addr constant [20 x i8] c"java/util/ArrayList\00", align 1
@.str.55 = private unnamed_addr constant [32 x i8] c"mono/android/runtime/JavaObject\00", align 1
@.str.56 = private unnamed_addr constant [35 x i8] c"android/runtime/JavaProxyThrowable\00", align 1
@.str.57 = private unnamed_addr constant [18 x i8] c"java/util/HashSet\00", align 1
@.str.58 = private unnamed_addr constant [41 x i8] c"mono/android/runtime/OutputStreamAdapter\00", align 1
@.str.59 = private unnamed_addr constant [24 x i8] c"android/graphics/Bitmap\00", align 1
@.str.60 = private unnamed_addr constant [24 x i8] c"android/graphics/Canvas\00", align 1
@.str.61 = private unnamed_addr constant [31 x i8] c"android/graphics/BitmapFactory\00", align 1
@.str.62 = private unnamed_addr constant [23 x i8] c"android/graphics/Color\00", align 1
@.str.63 = private unnamed_addr constant [23 x i8] c"android/graphics/Paint\00", align 1
@.str.64 = private unnamed_addr constant [29 x i8] c"android/graphics/Paint$Align\00", align 1
@.str.65 = private unnamed_addr constant [22 x i8] c"android/graphics/Path\00", align 1
@.str.66 = private unnamed_addr constant [22 x i8] c"android/graphics/Rect\00", align 1
@.str.67 = private unnamed_addr constant [24 x i8] c"android/content/Context\00", align 1
@.str.68 = private unnamed_addr constant [23 x i8] c"android/content/Intent\00", align 1
@.str.69 = private unnamed_addr constant [31 x i8] c"android/content/ContextWrapper\00", align 1
@.str.70 = private unnamed_addr constant [30 x i8] c"android/content/res/Resources\00", align 1
@.str.71 = private unnamed_addr constant [35 x i8] c"android/content/pm/ApplicationInfo\00", align 1
@.str.72 = private unnamed_addr constant [35 x i8] c"android/content/pm/PackageItemInfo\00", align 1
@.str.73 = private unnamed_addr constant [21 x i8] c"android/app/Activity\00", align 1
@.str.74 = private unnamed_addr constant [24 x i8] c"android/app/Application\00", align 1
@.str.75 = private unnamed_addr constant [26 x i8] c"java/net/ConnectException\00", align 1
@.str.76 = private unnamed_addr constant [27 x i8] c"java/net/HttpURLConnection\00", align 1
@.str.77 = private unnamed_addr constant [27 x i8] c"java/net/InetSocketAddress\00", align 1
@.str.78 = private unnamed_addr constant [27 x i8] c"java/net/ProtocolException\00", align 1
@.str.79 = private unnamed_addr constant [15 x i8] c"java/net/Proxy\00", align 1
@.str.80 = private unnamed_addr constant [20 x i8] c"java/net/Proxy$Type\00", align 1
@.str.81 = private unnamed_addr constant [23 x i8] c"java/net/SocketAddress\00", align 1
@.str.82 = private unnamed_addr constant [25 x i8] c"java/net/SocketException\00", align 1
@.str.83 = private unnamed_addr constant [32 x i8] c"java/net/SocketTimeoutException\00", align 1
@.str.84 = private unnamed_addr constant [33 x i8] c"java/net/UnknownServiceException\00", align 1
@.str.85 = private unnamed_addr constant [13 x i8] c"java/net/URL\00", align 1
@.str.86 = private unnamed_addr constant [23 x i8] c"java/net/URLConnection\00", align 1
@.str.87 = private unnamed_addr constant [18 x i8] c"java/security/Key\00", align 1
@.str.88 = private unnamed_addr constant [24 x i8] c"java/security/Principal\00", align 1
@.str.89 = private unnamed_addr constant [25 x i8] c"java/security/PrivateKey\00", align 1
@.str.90 = private unnamed_addr constant [25 x i8] c"java/security/KeyFactory\00", align 1
@.str.91 = private unnamed_addr constant [23 x i8] c"java/security/KeyStore\00", align 1
@.str.92 = private unnamed_addr constant [27 x i8] c"java/security/SecureRandom\00", align 1
@.str.93 = private unnamed_addr constant [34 x i8] c"java/security/spec/EncodedKeySpec\00", align 1
@.str.94 = private unnamed_addr constant [27 x i8] c"java/security/spec/KeySpec\00", align 1
@.str.95 = private unnamed_addr constant [39 x i8] c"java/security/spec/PKCS8EncodedKeySpec\00", align 1
@.str.96 = private unnamed_addr constant [31 x i8] c"java/security/cert/Certificate\00", align 1
@.str.97 = private unnamed_addr constant [30 x i8] c"java/nio/channels/FileChannel\00", align 1
@.str.98 = private unnamed_addr constant [51 x i8] c"java/nio/channels/spi/AbstractInterruptibleChannel\00", align 1
@.str.99 = private unnamed_addr constant [13 x i8] c"java/io/File\00", align 1
@.str.100 = private unnamed_addr constant [24 x i8] c"java/io/FileInputStream\00", align 1
@.str.101 = private unnamed_addr constant [20 x i8] c"java/io/InputStream\00", align 1
@.str.102 = private unnamed_addr constant [31 x i8] c"java/io/InterruptedIOException\00", align 1
@.str.103 = private unnamed_addr constant [20 x i8] c"java/io/IOException\00", align 1
@.str.104 = private unnamed_addr constant [21 x i8] c"java/io/Serializable\00", align 1
@.str.105 = private unnamed_addr constant [21 x i8] c"java/io/OutputStream\00", align 1
@.str.106 = private unnamed_addr constant [20 x i8] c"java/io/PrintWriter\00", align 1
@.str.107 = private unnamed_addr constant [21 x i8] c"java/io/StringWriter\00", align 1
@.str.108 = private unnamed_addr constant [15 x i8] c"java/io/Writer\00", align 1
@.str.109 = private unnamed_addr constant [22 x i8] c"java/util/Enumeration\00", align 1
@.str.110 = private unnamed_addr constant [19 x i8] c"java/util/Iterator\00", align 1
@.str.111 = private unnamed_addr constant [17 x i8] c"java/util/Random\00", align 1
@.str.112 = private unnamed_addr constant [28 x i8] c"java/util/function/Consumer\00", align 1
@.str.113 = private unnamed_addr constant [18 x i8] c"java/lang/Boolean\00", align 1
@.str.114 = private unnamed_addr constant [15 x i8] c"java/lang/Byte\00", align 1
@.str.115 = private unnamed_addr constant [20 x i8] c"java/lang/Character\00", align 1
@.str.116 = private unnamed_addr constant [16 x i8] c"java/lang/Class\00", align 1
@.str.117 = private unnamed_addr constant [33 x i8] c"java/lang/ClassNotFoundException\00", align 1
@.str.118 = private unnamed_addr constant [17 x i8] c"java/lang/Double\00", align 1
@.str.119 = private unnamed_addr constant [20 x i8] c"java/lang/Exception\00", align 1
@.str.120 = private unnamed_addr constant [16 x i8] c"java/lang/Float\00", align 1
@.str.121 = private unnamed_addr constant [23 x i8] c"java/lang/CharSequence\00", align 1
@.str.122 = private unnamed_addr constant [18 x i8] c"java/lang/Integer\00", align 1
@.str.123 = private unnamed_addr constant [15 x i8] c"java/lang/Long\00", align 1
@.str.124 = private unnamed_addr constant [17 x i8] c"java/lang/Object\00", align 1
@.str.125 = private unnamed_addr constant [27 x i8] c"java/lang/RuntimeException\00", align 1
@.str.126 = private unnamed_addr constant [16 x i8] c"java/lang/Short\00", align 1
@.str.127 = private unnamed_addr constant [17 x i8] c"java/lang/String\00", align 1
@.str.128 = private unnamed_addr constant [17 x i8] c"java/lang/Thread\00", align 1
@.str.129 = private unnamed_addr constant [35 x i8] c"mono/java/lang/RunnableImplementor\00", align 1
@.str.130 = private unnamed_addr constant [20 x i8] c"java/lang/Throwable\00", align 1
@.str.131 = private unnamed_addr constant [29 x i8] c"java/lang/ClassCastException\00", align 1
@.str.132 = private unnamed_addr constant [15 x i8] c"java/lang/Enum\00", align 1
@.str.133 = private unnamed_addr constant [16 x i8] c"java/lang/Error\00", align 1
@.str.134 = private unnamed_addr constant [35 x i8] c"java/lang/IllegalArgumentException\00", align 1
@.str.135 = private unnamed_addr constant [32 x i8] c"java/lang/IllegalStateException\00", align 1
@.str.136 = private unnamed_addr constant [36 x i8] c"java/lang/IndexOutOfBoundsException\00", align 1
@.str.137 = private unnamed_addr constant [19 x i8] c"java/lang/Runnable\00", align 1
@.str.138 = private unnamed_addr constant [17 x i8] c"java/lang/System\00", align 1
@.str.139 = private unnamed_addr constant [23 x i8] c"java/lang/LinkageError\00", align 1
@.str.140 = private unnamed_addr constant [31 x i8] c"java/lang/NoClassDefFoundError\00", align 1
@.str.141 = private unnamed_addr constant [31 x i8] c"java/lang/NullPointerException\00", align 1
@.str.142 = private unnamed_addr constant [17 x i8] c"java/lang/Number\00", align 1
@.str.143 = private unnamed_addr constant [39 x i8] c"java/lang/ReflectiveOperationException\00", align 1
@.str.144 = private unnamed_addr constant [28 x i8] c"java/lang/SecurityException\00", align 1
@.str.145 = private unnamed_addr constant [28 x i8] c"java/lang/StackTraceElement\00", align 1
@.str.146 = private unnamed_addr constant [40 x i8] c"java/lang/UnsupportedOperationException\00", align 1
@.str.147 = private unnamed_addr constant [25 x i8] c"mono/android/TypeManager\00", align 1
@.str.148 = private unnamed_addr constant [35 x i8] c"crc64f7520a812a03248f/MainActivity\00", align 1
@.str.149 = private unnamed_addr constant [43 x i8] c"crc64f7520a812a03248f/SelectFolderActivity\00", align 1
@.str.150 = private unnamed_addr constant [38 x i8] c"crc64f7520a812a03248f/SetFontActivity\00", align 1
@.str.151 = private unnamed_addr constant [35 x i8] c"EraAndroid/FrontEnd/EmueraFrontEnd\00", align 1

;TypeMapModule
@.TypeMapModule.0_assembly_name = private unnamed_addr constant [13 x i8] c"EraAndroid64\00", align 1
@.TypeMapModule.1_assembly_name = private unnamed_addr constant [13 x i8] c"Mono.Android\00", align 1

; Metadata
!llvm.module.flags = !{!0, !1, !7, !8, !9, !10}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!".NET for Android remotes/origin/release/9.0.1xx @ 9abff7703206541fdb83ffa80fe2c2753ad1997b"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"branch-target-enforcement", i32 0}
!8 = !{i32 1, !"sign-return-address", i32 0}
!9 = !{i32 1, !"sign-return-address-all", i32 0}
!10 = !{i32 1, !"sign-return-address-with-bkey", i32 0}
