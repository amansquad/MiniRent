import { NextResponse } from "next/server";

export async function GET(
    request: Request,
    { params }: { params: Promise<{ id: string }> }
) {
    const { id } = await params;
    // Images are included in the property details response
    // This endpoint is only for POST (adding images)
    return NextResponse.json(
        { message: "Images are included in the property details. Use GET /api/properties/{id} instead." },
        { status: 400 }
    );
}

export async function POST(
    request: Request,
    { params }: { params: Promise<{ id: string }> }
) {
    const { id } = await params;
    const token = request.headers.get("authorization") || "";
    const body = await request.json();

    const res = await fetch(`http://127.0.0.1:5000/api/properties/${id}/images`, {
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
