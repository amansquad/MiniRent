import { NextResponse } from "next/server";

export async function POST(request: Request) {
    const token = request.headers.get("authorization") || "";
    const body = await request.json();

    const res = await fetch("http://127.0.0.1:5000/api/reviews", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { Authorization: token } : {}),
        },
        body: JSON.stringify(body),
    });

    if (!res.ok) {
        const error = await res.json();
        return NextResponse.json(error, { status: res.status });
    }

    const data = await res.json();
    return NextResponse.json(data);
}
