using Android.Content;
using Android.Net;
using Android.Provider;
using EmueraFramework;
using System;
using System.IO;


namespace EraAndroid
{
    public static class EraAndroidFileProvider
    {
        // SAF에서 선택한 원본 URI
        public static Android.Net.Uri SelectedUri;

        // 앱 내부 캐시 경로 (Emuera_xxx)
        public static string LocalRootPath;

        // Context (ContentResolver 사용용)
        public static Context AppContext;

        // 앱 화면에 현재 파일 처리 상태를 표시하기 위한 콜백이다.
        public static Action<string> StatusCallback;

        // SAF에서 실제로 확인한 파일 개수를 기록한다.
        private static int ensureCount = 0;

        // SAF에서 실제로 복사한 파일 개수를 기록한다.
        private static int copiedCount = 0;

        /// 현재 파일 처리 상태를 앱 화면에 표시한다.
        public static void ReportStatus(string message)
        {
            StatusCallback?.Invoke(message);
        }

        /// 파일이 없으면 SAF에서 해당 파일만 가져온다
        public static void EnsureFileExists(string targetPath)
        {
            try
            {
                // 이미 있으면 아무것도 하지 않는다.
                if (File.Exists(targetPath))
                    return;

                // 실제로 없는 파일만 SAF에서 찾기 위해 확인 개수를 증가시킨다.
                ensureCount++;
                Android.Util.Log.Debug("SAF", $"Ensure [{ensureCount}]: {targetPath}");
                FileLog.Info("SAF Lazy Copy", "Ensure [" + ensureCount + "]: " + targetPath);

                StatusCallback?.Invoke("없는 파일 확인 중: " + ensureCount + "개 확인");

                // 상대 경로 계산
                string relativePath = GetRelativePath(targetPath, LocalRootPath);

                // SAF에서 해당 파일 찾기
                Android.Net.Uri fileUri = FindFileUri(SelectedUri, relativePath);

                if (fileUri == null)
                    return;

                // 폴더 생성
                var dir = Path.GetDirectoryName(targetPath);
                if (!string.IsNullOrEmpty(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                // 스트림 복사
                using var input = AppContext.ContentResolver.OpenInputStream(fileUri);
                using var output = new FileStream(targetPath, FileMode.Create, FileAccess.Write);

                input.CopyTo(output);

                // SAF에서 파일을 실제로 복사했을 때 개수를 증가시키고 로그로 남긴다.
                copiedCount++;

                Android.Util.Log.Debug("SAF", $"Copied [{copiedCount}]: {relativePath}");
                FileLog.Info("SAF Lazy Copy", "Copied [" + copiedCount + "]: " + relativePath);

                StatusCallback?.Invoke("필요한 파일 복사 중: " + copiedCount + "개 복사");
            }
            catch
            {
                // 실패해도 기존 파일 읽기 흐름을 방해하지 않는다.
            }
        }

        /// 내부 경로 → 상대 경로 변환
        private static string GetRelativePath(string fullPath, string rootPath)
        {
            if (!fullPath.StartsWith(rootPath))
                return fullPath;

            string rel = fullPath.Substring(rootPath.Length);
            return rel.TrimStart(Path.DirectorySeparatorChar);
        }

        /// SAF에서 상대 경로 파일 찾기
        private static Android.Net.Uri FindFileUri(Android.Net.Uri rootUri, string relativePath)
        {
            try
            {
                string[] parts = relativePath
                    .Replace('\\', '/')
                    .Split('/');
                Android.Net.Uri current = rootUri;

                foreach (var part in parts)
                {
                    if (string.IsNullOrEmpty(part))
                        continue;

                    current = FindChild(current, part);

                    if (current == null)
                        return null;
                }

                return current;
            }
            catch
            {
                return null;
            }
        }

        /// SAF에서 자식 파일 찾기
        private static Android.Net.Uri FindChild(Android.Net.Uri parentUri, string name)
        {
            var childrenUri = DocumentsContract.BuildChildDocumentsUriUsingTree(
                parentUri,
                DocumentsContract.GetDocumentId(parentUri)
            );

            using var cursor = AppContext.ContentResolver.Query(
                childrenUri,
                new string[]
                {
                    DocumentsContract.Document.ColumnDocumentId,
                    DocumentsContract.Document.ColumnDisplayName
                },
                null,
                null,
                null
            );

            while (cursor.MoveToNext())
            {
                string docId = cursor.GetString(0);
                string displayName = cursor.GetString(1);

                if (displayName == name)
                {
                    return DocumentsContract.BuildDocumentUriUsingTree(parentUri, docId);
                }
            }

            return null;
        }
    }
}