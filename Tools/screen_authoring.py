#!/usr/bin/env python3
"""Submit a JSON screen-authoring request to an open Unity Editor."""

import argparse
import json
from pathlib import Path
import sys
import time
import uuid


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--project", required=True, type=Path, help="Explicit Unity project root")
    source = parser.add_mutually_exclusive_group(required=True)
    source.add_argument("--request", type=Path, help="Request JSON file to submit")
    source.add_argument("--result", help="Previously returned request ID; read without resubmitting")
    parser.add_argument("--timeout", type=float, default=30, help="Wait seconds, 0 to 60 (default: 30)")
    args = parser.parse_args()
    if not 0 <= args.timeout <= 60:
        parser.error("--timeout must be between 0 and 60")
    project = args.project.resolve()
    if not (project / "ProjectSettings/ProjectVersion.txt").is_file():
        parser.error("--project must identify a Unity project")
    root = project / "Library/ScreenAuthoring"
    request_id = args.result
    queued = None
    if args.request:
        request_text = args.request.read_text(encoding="utf-8")
        payload = json.loads(request_text)
        if not isinstance(payload, dict):
            parser.error("Request must be a JSON object")
        request_id = f"{time.time_ns():020d}_{uuid.uuid4().hex}"
        inbox = root / "Inbox"
        inbox.mkdir(parents=True, exist_ok=True)
        queued = inbox / f"{request_id}.json"
        staging = queued.with_suffix(".tmp")
        staging.write_text(request_text, encoding="utf-8")
        staging.rename(queued)
    elif not request_id or any(c not in "0123456789abcdef_" for c in request_id):
        parser.error("Invalid request ID")

    result_path = root / "Results" / f"{request_id}.json"
    deadline = time.monotonic() + args.timeout
    while True:
        if result_path.is_file():
            result = json.loads(result_path.read_text(encoding="utf-8"))
            result["requestId"] = request_id
            result["resultFile"] = str(result_path)
            print(json.dumps(result, ensure_ascii=False, indent=2))
            return 0 if result["success"] else 1
        if time.monotonic() >= deadline:
            break
        time.sleep(0.2)

    status = "pending"
    # A zero timeout intentionally submits without waiting. Otherwise cancel only
    # requests the Editor has not claimed, so they cannot execute unexpectedly later.
    if queued is not None and args.timeout > 0:
        try:
            queued.unlink()
            status = "cancelled-before-execution"
        except FileNotFoundError:
            pass
    print(json.dumps({
        "status": status,
        "requestId": request_id,
        "resultFile": str(result_path),
        "message": (
            "The Editor did not claim this request. Check compilation/Auto Refresh, then submit again."
            if status == "cancelled-before-execution" else
            "Read with --result using this request ID. Do not resubmit a mutation while it is pending."
        ),
    }, indent=2))
    return 2 if status == "cancelled-before-execution" else 3


if __name__ == "__main__":
    try:
        sys.exit(main())
    except (OSError, ValueError) as error:
        print(str(error), file=sys.stderr)
        sys.exit(1)
