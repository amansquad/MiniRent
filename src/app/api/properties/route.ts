import { NextResponse } from "next/server";

export async function GET(request: Request) {
    // Proxy to backend with JWT
    const { searchParams } = new URL(request.url);
    const mode = searchParams.get("mode");
    const query = mode ? `?mode=${mode}` : "";

    const token = request.headers.get("authorization") || "";
    const res = await fetch(`http://127.0.0.1:5000/api/properties${query}`, {
        cache: "no-store",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { "Authorization": token } : {})
        }
    });
    if (!res.ok) {
        return NextResponse.json({ error: "Failed to fetch properties" }, { status: res.status });
    }
    const data = await res.json();
    return NextResponse.json(data);
}

export async function POST(request: Request) {
    const token = request.headers.get("authorization") || "";
    const body = await request.json();

    const res = await fetch("http://127.0.0.1:5000/api/properties", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { "Authorization": token } : {})
        },
        body: JSON.stringify(body)
    });

    if (!res.ok) {
        const error = await res.text();
        return NextResponse.json({ error }, { status: res.status });
    }

    const data = await res.json();
    return NextResponse.json(data, { status: 201 });
}
