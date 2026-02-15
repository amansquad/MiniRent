import { NextResponse } from "next/server";

export async function GET(request: Request) {
    const token = request.headers.get("authorization") || "";
    const res = await fetch("http://127.0.0.1:5000/api/users", {
        cache: "no-store",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { "Authorization": token } : {})
        }
    });
    if (!res.ok) {
        return NextResponse.json({ error: "Failed to fetch users" }, { status: res.status });
    }
    const data = await res.json();
    return NextResponse.json(data);
}
