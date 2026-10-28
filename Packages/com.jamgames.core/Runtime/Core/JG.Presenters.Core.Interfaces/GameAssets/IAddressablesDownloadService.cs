using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.JG.Presenters.Core.Interfaces.GameAssets
{
    /// <summary>
    /// Service for downloading Addressables content by label(s)/tag(s) with size, progress and speed reporting.
    /// </summary>
    public interface IAddressablesDownloadService
    {
        /// <summary>
        /// Returns the total number of bytes that would need to be downloaded for the given label/tag.
        /// </summary>
        UniTask<long> GetDownloadSizeAsync(string label, CancellationToken ct = default);

        /// <summary>
        /// Returns the total number of bytes that would need to be downloaded for the union of all provided labels/tags.
        /// </summary>
        UniTask<long> GetDownloadSizeAsync(IList<string> labels, CancellationToken ct = default);

        /// <summary>
        /// Downloads all dependencies for the given label/tag. Reports progress (bytes, percent) and speed.
        /// Completes when the download finishes or throws on failure/cancellation.
        /// </summary>
        UniTask DownloadByLabelAsync(string label, IProgress<AddressablesDownloadProgress> progress = null, CancellationToken ct = default);

        /// <summary>
        /// Downloads all dependencies for the union of provided labels/tags. Reports progress (bytes, percent) and speed.
        /// Completes when the download finishes or throws on failure/cancellation.
        /// </summary>
        UniTask DownloadByLabelsAsync(IList<string> labels, IProgress<AddressablesDownloadProgress> progress = null, CancellationToken ct = default);

        /// <summary>
        /// Returns true if all bundles required by the given label are already cached for the current catalogs.
        /// Optionally checks and updates catalogs first to account for server-side content updates.
        /// </summary>
        UniTask<bool> IsFullyCachedAsync(string label, bool checkForCatalogUpdates = true, CancellationToken ct = default);

        /// <summary>
        /// Returns true if all bundles required by the union of labels are already cached for the current catalogs.
        /// Optionally checks and updates catalogs first to account for server-side content updates.
        /// </summary>
        UniTask<bool> IsFullyCachedAsync(IList<string> labels, bool checkForCatalogUpdates = true, CancellationToken ct = default);
    }

    /// <summary>
    /// Progress DTO for Addressables downloads.
    /// </summary>
    public struct AddressablesDownloadProgress
    {
        public string LabelOrSummary; // label for single or summary text for multiple
        public long DownloadedBytes;
        public long TotalBytes;
        public float Progress01; // 0..1
        public double BytesPerSecond; // current speed estimate
        public TimeSpan Elapsed;
    }
}
