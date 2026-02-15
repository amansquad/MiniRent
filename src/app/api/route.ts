import { NextResponse } from "next/server";

// Proxy to backend API for dashboard overview
export async function GET(request: Request) {
  const token = request.headers.get("authorization") || "";
  try {
    const res = await fetch("http://127.0.0.1:5000/api/dashboard/overview", {
      headers: {
        "Content-Type": "application/json",
        ...(token ? { Authorization: token } : {}),
      },
      cache: "no-store",
    });

    const rawBody = await res.text();
    let parsedBody: any = null;
    try {
      parsedBody = rawBody ? JSON.parse(rawBody) : null;
    } catch {
      // non‑JSON body, keep as text
    }

    if (!res.ok) {
      const message =
        (parsedBody && (parsedBody.message || parsedBody.error)) ||
        rawBody ||
        "Failed to fetch dashboard data";

      return NextResponse.json(
        { error: "Failed to fetch dashboard data", details: message },
        { status: res.status }
      );
    }

    const data = parsedBody ?? {};
    return NextResponse.json(data);
  } catch (err: any) {
    console.error("Dashboard proxy error:", err);
    return NextResponse.json(
      { error: "Failed to fetch dashboard data", details: String(err?.message ?? err) },
      { status: 500 }
    );
  }
}