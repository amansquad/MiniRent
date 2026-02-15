import { NextResponse } from "next/server";

export async function GET(request: Request, context: { params: Promise<{ id: string }> }) {
    const { id } = await context.params;
    const token = request.headers.get("authorization") || "";

    const res = await fetch(`http://127.0.0.1:5000/api/properties/${id}`, {
        headers: {
            "Content-Type": "application/json",
            ...(token ? { "Authorization": token } : {})
        }
    });

    if (!res.ok) {
        const error = await res.text();
        return NextResponse.json({ error }, { status: res.status });
    }

    const data = await res.json();
    return NextResponse.json(data);
}

export async function PUT(request: Request, context: { params: Promise<{ id: string }> }) {
    const { id } = await context.params;
    const token = request.headers.get("authorization") || "";
    let body;
    try {
        body = await request.json();
    } catch (e) {
        return NextResponse.json({ error: "Invalid JSON body" }, { status: 400 });
    }

    const res = await fetch(`http://127.0.0.1:5000/api/properties/${id}`, {
        method: "PUT",
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
    return NextResponse.json(data);
}

export async function DELETE(request: Request, context: { params: Promise<{ id: string }> }) {
    const { id } = await context.params;
    const token = request.headers.get("authorization") || "";

    const res = await fetch(`http://127.0.0.1:5000/api/properties/${id}`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
            ...(token ? { "Authorization": token } : {})
        }
    });

    if (!res.ok) {
        const error = await res.text();
        return NextResponse.json({ error }, { status: res.status });
    }

    if (res.status === 204) {
        return new NextResponse(null, { status: 204 });
    }

    const data = await res.json();
    return NextResponse.json(data);
}
