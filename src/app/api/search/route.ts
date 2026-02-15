import { NextResponse } from "next/server";

export async function GET(request: Request) {
    try {
        const { searchParams } = new URL(request.url);
        const query = searchParams.get("q") || "";
        const token = request.headers.get("authorization") || "";

        const res = await fetch(`http://127.0.0.1:5000/api/search?q=${encodeURIComponent(query)}`, {
            cache: "no-store",
            headers: {
                "Content-Type": "application/json",
                ...(token ? { "Authorization": token } : {})
            }
        });

        if (!res.ok) {
            return NextResponse.json({ error: "Search failed" }, { status: res.status });
        }

        const data = await res.json();
        return NextResponse.json(data);
    } catch (error: any) {
        console.error("Search proxy error:", error);
        return NextResponse.json({ error: "Internal server error" }, { status: 500 });
    }
}
