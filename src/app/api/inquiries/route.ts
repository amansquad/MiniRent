import { NextResponse } from "next/server";

export async function GET(request: Request) {
    const token = request.headers.get("authorization") || "";
    const res = await fetch("http://127.0.0.1:5000/api/inquiries", {
        cache: "no-store",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { "Authorization": token } : {})
        }
    });
    if (!res.ok) {
        return NextResponse.json({ error: "Failed to fetch inquiries" }, { status: res.status });
    }
    const data = await res.json();
    return NextResponse.json(data);
}

export async function POST(request: Request) {
    const token = request.headers.get("authorization") || "";
    const body = await request.json();

    const res = await fetch("http://127.0.0.1:5000/api/inquiries", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { "Authorization": token } : {})
        },
        body: JSON.stringify(body)
    });

    if (!res.ok) {
        const errorData: any = await res.json().catch(() => ({}));
        return NextResponse.json(
            { error: errorData.message || "Failed to create inquiry" },
            { status: res.status }
        );
    }

    const data = await res.json();
    return NextResponse.json(data, { status: 201 });
}
