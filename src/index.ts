import express from 'express';
import cors from 'cors';
import helmet from 'helmet';
import rateLimit from 'express-rate-limit';
import dotenv from 'dotenv';
import { PrismaClient } from '@prisma/client';
// Note: this repository primarily contains a Next.js frontend and a .NET backend.
// The following imports referenced an Express API server scaffold which is not
// present in this workspace. Keep this file minimal to avoid runtime errors.
// If you intend to run a separate Node/Express API, restore or implement
// these modules under `src/routes` and `src/middleware`.

// import { logger } from './utils/logger';
// import { errorHandler } from './middleware/errorHandler';
// import { notFoundHandler } from './middleware/notFoundHandler';

// Routes (not present in this repo)
// import authRoutes from './routes/auth';
// import userRoutes from './routes/users';
// import propertyRoutes from './routes/properties';
// import rentalRoutes from './routes/rentals';
// import inquiryRoutes from './routes/inquiries';
// import dashboardRoutes from './routes/dashboard';
// import utilityRoutes from './routes/utilities';

// Load environment variables
dotenv.config();

const app = express();
const PORT = process.env.PORT || 5000;

// Initialize Prisma Client
export const prisma = new PrismaClient({
  log: [
    {
      emit: 'event',
      level: 'query',
    },
    {
      emit: 'event',
      level: 'error',
    },
    {
      emit: 'event',
      level: 'info',
    },
    {
      emit: 'event',
      level: 'warn',
    },
  ],
});

// Log Prisma events
prisma.$on('query', (e) => {
  logger.debug('Query: ' + e.query);
  logger.debug('Params: ' + e.params);
  logger.debug('Duration: ' + e.duration + 'ms');
});

prisma.$on('error', (e) => {
  logger.error('Prisma Error: ' + e.message);
});

prisma.$on('info', (e) => {
  logger.info('Prisma Info: ' + e.message);
});

prisma.$on('warn', (e) => {
  logger.warn('Prisma Warning: ' + e.message);
});

// Security middleware
app.use(helmet());

// CORS configuration
app.use(cors({
  origin: process.env.FRONTEND_URL || 'http://localhost:3000',
  credentials: true,
}));

// Rate limiting
const limiter = rateLimit({
  windowMs: parseInt(process.env.RATE_LIMIT_WINDOW_MS || '900000'), // 15 minutes
  max: parseInt(process.env.RATE_LIMIT_MAX_REQUESTS || '100'), // limit each IP to 100 requests per windowMs
  message: {
    error: 'Too many requests from this IP, please try again later.',
  },
  standardHeaders: true,
  legacyHeaders: false,
});

app.use('/api/', limiter);

// Body parsing middleware
app.use(express.json({ limit: '10mb' }));
app.use(express.urlencoded({ extended: true, limit: '10mb' }));

// Request logging middleware
app.use((req, res, next) => {
  logger.info(`${req.method} ${req.path} - ${req.ip}`);
  next();
});

// Health check endpoint
app.get('/health', (req, res) => {
  res.status(200).json({
    status: 'OK',
    timestamp: new Date().toISOString(),
    uptime: process.uptime(),
  });
});

// API routes would be mounted here if an Express routes implementation existed.
// Example (requires implementing the routes):
// app.use('/api/auth', authRoutes);

// Error handling middleware would be used here if present.
// app.use(notFoundHandler);
// app.use(errorHandler);

// Graceful shutdown
process.on('SIGTERM', async () => {
  logger.info('SIGTERM received, shutting down gracefully');
  await prisma.$disconnect();
  process.exit(0);
});

process.on('SIGINT', async () => {
  logger.info('SIGINT received, shutting down gracefully');
  await prisma.$disconnect();
  process.exit(0);
});

// Start server
const startServer = async () => {
  try {
    // Test database connection
    await prisma.$connect();
    logger.info('Database connected successfully');

    app.listen(PORT, () => {
      logger.info(`Server is running on port ${PORT}`);
      logger.info(`Environment: ${process.env.NODE_ENV}`);
    });
  } catch (error) {
    logger.error('Failed to start server:', error);
    process.exit(1);
  }
};

startServer();
