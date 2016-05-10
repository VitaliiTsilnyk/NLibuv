using System;

namespace NLibuv
{
	public enum UvLoopRunMode
	{
		/// <summary>
		/// Runs the event loop until there are no more active and referenced handles or requests.
		/// Returns non-zero if <see cref="UvLoop.Stop"/> was called and there are still active handles or requests.
		/// Returns zero in all other cases.
		/// </summary>
		RunDefault = 0,

		/// <summary>
		/// Poll for i/o once. Note that this function blocks if there are no pending callbacks.
		/// Returns zero when done (no active handles or requests left), or non-zero if more callbacks
		/// are expected (meaning you should run the event loop again sometime in the future).
		/// </summary>
		RunOnce,

		/// <summary>
		/// Poll for i/o once but don’t block if there are no pending callbacks.
		/// Returns zero if done (no active handles or requests left), or non-zero if more callbacks
		/// are expected (meaning you should run the event loop again sometime in the future).
		/// </summary>
		RunNowait,
	}
}