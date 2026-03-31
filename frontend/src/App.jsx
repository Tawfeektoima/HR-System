import { Routes, Route, Navigate } from 'react-router-dom';
import PublicLayout from './components/layout/PublicLayout';
import HRLayout from './components/layout/HRLayout';
import ProtectedRoute from './components/auth/ProtectedRoute';

import JobsListPage from './pages/public/JobsListPage';
import DashboardPage from './pages/hr/DashboardPage';
import LoginPage from './pages/auth/LoginPage';
import ApplyPage from './pages/public/ApplyPage';
import ATSBoardPage from './pages/hr/ATSBoardPage';
import HRJobsPage from './pages/hr/HRJobsPage';
import CandidatesPage from './pages/hr/CandidatesPage';
import InterviewsPage from './pages/hr/InterviewsPage';

function App() {
  return (
    <Routes>
      {/* Public Facing Routes */}
      <Route path="/" element={<PublicLayout />}>
        <Route index element={<JobsListPage />} />
        <Route path="jobs/:id/apply" element={<ApplyPage />} />
      </Route>

      {/* Auth Route */}
      <Route path="/login" element={<LoginPage />} />

      {/* HR Protected Routes */}
      <Route path="/hr" element={<ProtectedRoute><HRLayout /></ProtectedRoute>}>
        <Route index element={<Navigate to="/hr/dashboard" replace />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="jobs" element={<HRJobsPage />} />
        <Route path="candidates" element={<CandidatesPage />} />
        <Route path="interviews" element={<InterviewsPage />} />
        <Route path="applications" element={<ATSBoardPage />} />
      </Route>
      
      {/* Catch-all */}
      <Route path="*" element={<div className="p-8 text-center text-danger">404 - Page not found</div>} />
    </Routes>
  );
}

export default App;
