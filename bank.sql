-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Jun 19, 2024 at 11:19 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `bank`
--

-- --------------------------------------------------------

--
-- Table structure for table `moneyrequests`
--

CREATE TABLE `moneyrequests` (
  `RequestId` int(11) NOT NULL,
  `SenderAccountNumber` varchar(20) NOT NULL,
  `RecipientAccountNumber` varchar(20) NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `Description` text DEFAULT NULL,
  `RequestStatus` enum('Pending','Approved','Declined') DEFAULT 'Pending',
  `CreatedAt` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `transactions`
--

CREATE TABLE `transactions` (
  `TransactionId` int(11) NOT NULL,
  `AccountName` varchar(100) NOT NULL,
  `AccountNumber` varchar(20) NOT NULL,
  `RecipientAccountNumber` varchar(20) DEFAULT NULL,
  `RequestStatus` varchar(50) DEFAULT NULL,
  `TransactionType` varchar(50) NOT NULL,
  `Amount` decimal(18,2) NOT NULL,
  `TransactionDate` datetime NOT NULL DEFAULT current_timestamp(),
  `Description` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `transactions`
--

INSERT INTO `transactions` (`TransactionId`, `AccountName`, `AccountNumber`, `RecipientAccountNumber`, `RequestStatus`, `TransactionType`, `Amount`, `TransactionDate`, `Description`) VALUES
(1, 'juan@123', '1234567891', NULL, NULL, 'Deposit', 100.00, '2024-06-17 21:44:40', 'Deposit'),
(2, 'juan@123', '1234567891', NULL, NULL, 'Send', -11.00, '2024-06-17 21:45:21', 'Desc here'),
(3, 'maria@123', '1234567892', NULL, NULL, 'Receive', 11.00, '2024-06-17 21:45:21', 'Desc here'),
(4, 'juan@123', '1234567891', NULL, NULL, 'Withdrawal', -10.00, '2024-06-17 21:56:37', 'Withdrawal'),
(5, 'juan@123', '1234567891', NULL, NULL, 'Withdrawal', -10.00, '2024-06-17 21:59:56', 'Withdrawal'),
(6, 'juan@123', '1234567891', NULL, NULL, 'Send', -11.00, '2024-06-17 22:01:49', 'Desc here'),
(7, 'maria@123', '1234567892', NULL, NULL, 'Receive', 11.00, '2024-06-17 22:01:49', 'Desc here'),
(8, 'juan@123', '1234567891', NULL, NULL, 'Send', -11.00, '2024-06-18 08:33:01', 'Desc here'),
(9, 'maria@123', '1234567892', NULL, NULL, 'Receive', 11.00, '2024-06-18 08:33:01', 'Desc here'),
(10, 'juan@123', '1234567891', NULL, NULL, 'Send', -11.00, '2024-06-18 09:00:31', 'Desc here'),
(11, 'maria@123', '1234567892', NULL, NULL, 'Receive', 11.00, '2024-06-18 09:00:31', 'Desc here'),
(12, 'juan@123', '1234567891', NULL, NULL, 'Withdrawal', -10.00, '2024-06-18 09:01:02', 'Withdrawal'),
(13, 'juan@123', '1234567891', NULL, NULL, 'Request', 10.00, '2024-06-18 12:44:49', 'Request Money'),
(14, 'juan@123', '1234567891', '1234567892', 'Accepted', 'Request', 10.00, '2024-06-18 13:09:09', 'Request Money'),
(15, 'maria@123', '1234567892', NULL, NULL, 'Deposit', 40.00, '2024-06-18 15:05:24', 'Deposit'),
(16, 'juan@123', '1234567891', '1234567892', 'Rejected', 'Request', 100.00, '2024-06-18 16:03:10', 'Request Please'),
(17, 'juan@123', '1234567891', '1234567892', 'Accepted', 'Request', 10.00, '2024-06-18 16:24:28', 'Request Money'),
(18, 'juan@123', '1234567891', NULL, NULL, 'Send', -10.00, '2024-06-18 16:41:29', 'Send Money here'),
(19, 'maria@123', '1234567892', NULL, NULL, 'Receive', 10.00, '2024-06-18 16:41:29', 'Send Money here'),
(20, 'juan@123', '1234567891', NULL, NULL, 'Deposit', 10.00, '2024-06-18 16:43:25', 'Deposit'),
(21, 'juan@123', '1234567891', NULL, NULL, 'Deposit', 10.00, '2024-06-18 16:44:59', 'Deposit'),
(22, 'juan@123', '1234567891', NULL, NULL, 'Deposit', 30.00, '2024-06-18 16:45:09', 'Deposit'),
(23, 'maria@123', '1234567892', NULL, NULL, 'Deposit', 10.00, '2024-06-18 16:53:15', 'Deposit'),
(24, 'maria@123', '1234567892', NULL, NULL, 'Withdrawal', -10.00, '2024-06-18 16:53:45', 'Withdrawal'),
(25, 'juan@123', '1234567891', NULL, NULL, 'Withdrawal', -10.00, '2024-06-18 17:02:55', 'Withdrawal'),
(26, 'mark@123', '1234567894', NULL, NULL, 'Deposit', 100.00, '2024-06-18 17:28:47', 'Deposit'),
(27, 'mark@123', '1234567894', NULL, NULL, 'Send', -10.00, '2024-06-18 17:31:07', 'Send Money Here'),
(28, 'maria@123', '1234567892', NULL, NULL, 'Receive', 10.00, '2024-06-18 17:31:07', 'Send Money Here'),
(29, 'mark@123', '1234567894', NULL, NULL, 'Withdrawal', -10.00, '2024-06-18 17:32:13', 'Withdrawal'),
(30, 'mark@123', '1234567894', '1234567892', 'Accepted', 'Request', 20.00, '2024-06-18 17:33:12', 'Description for Request Money'),
(31, 'aris@gmail.com', '1234567895', NULL, NULL, 'Deposit', 100.00, '2024-06-18 17:39:06', 'Deposit'),
(32, 'aris@gmail.com', '1234567895', NULL, NULL, 'Send', -10.00, '2024-06-18 17:40:01', 'Send Money'),
(33, 'maria@123', '1234567892', NULL, NULL, 'Receive', 10.00, '2024-06-18 17:40:01', 'Send Money'),
(34, 'aris@gmail.com', '1234567895', NULL, NULL, 'Withdrawal', -10.00, '2024-06-18 17:40:44', 'Withdrawal'),
(35, 'aris@gmail.com', '1234567895', '1234567892', 'Accepted', 'Request', 10.00, '2024-06-18 17:41:23', 'Request Money Description'),
(36, 'juan@123', '1234567891', '1234567892', 'Accepted', 'Request', 10.00, '2024-06-19 22:15:41', 'Request Money po '),
(37, 'juan@123', '1234567891', NULL, NULL, 'Send', -10.00, '2024-06-19 22:41:14', 'Send Money Here'),
(38, 'maria@123', '1234567892', NULL, NULL, 'Receive', 10.00, '2024-06-19 22:41:14', 'Send Money Here'),
(39, 'juan@123', '1234567891', NULL, NULL, 'Deposit', 100.00, '2024-06-19 22:41:32', 'Deposit'),
(40, 'juan@123', '1234567891', NULL, NULL, 'Send', -10.00, '2024-06-19 22:43:18', 'Send Money Here'),
(41, 'maria@123', '1234567892', NULL, NULL, 'Receive', 10.00, '2024-06-19 22:43:18', 'Send Money Here'),
(42, 'juan@123', '1234567891', NULL, NULL, 'Send', -10.00, '2024-06-19 22:45:31', 'Send Money Here'),
(43, 'maria@123', '1234567892', NULL, NULL, 'Receive', 10.00, '2024-06-19 22:45:31', 'Send Money Here'),
(44, 'alvin@123', '1234567899', NULL, NULL, 'Deposit', 100.00, '2024-06-19 23:00:18', 'Deposit'),
(45, 'alvin@123', '1234567899', NULL, NULL, 'Send', -10.00, '2024-06-19 23:02:06', 'Send Moenyyy'),
(46, 'maria@123', '1234567892', NULL, NULL, 'Receive', 10.00, '2024-06-19 23:02:06', 'Send Moenyyy'),
(47, 'alvin@123', '1234567899', NULL, NULL, 'Withdrawal', -10.00, '2024-06-19 23:03:05', 'Withdrawal'),
(48, 'alvin@123', '1234567899', '1234567892', 'Accepted', 'Request', 10.00, '2024-06-19 23:04:00', 'Request Moneyyyy');

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `Id` int(11) NOT NULL,
  `Username` varchar(255) NOT NULL,
  `Password` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `DateOfBirth` date NOT NULL,
  `ContactNumber` varchar(15) NOT NULL,
  `Contact_verified` tinyint(1) NOT NULL DEFAULT 0,
  `Email` varchar(255) NOT NULL,
  `Address` text NOT NULL,
  `Branch` varchar(255) NOT NULL,
  `AccountType` varchar(255) NOT NULL,
  `AccountNumber` varchar(255) NOT NULL,
  `Balance` decimal(10,2) NOT NULL DEFAULT 0.00,
  `Pin` varchar(255) NOT NULL,
  `SecurityQuestion1` text NOT NULL,
  `SecurityQuestion2` text NOT NULL,
  `Answer1` text NOT NULL,
  `Answer2` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`Id`, `Username`, `Password`, `Name`, `DateOfBirth`, `ContactNumber`, `Contact_verified`, `Email`, `Address`, `Branch`, `AccountType`, `AccountNumber`, `Balance`, `Pin`, `SecurityQuestion1`, `SecurityQuestion2`, `Answer1`, `Answer2`) VALUES
(1, 'juan@123', 'qwerty2022', 'Juan Dela Cruz', '1997-05-09', '+639203614521', 1, 'juandelacruz@gmail.com', 'Address Here', 'Branch', 'deposit', '1234567891', 126.00, '1021', 'question1', 'question3', 'Maiden Name', 'Oppenheimer'),
(2, 'maria@123', 'qwerty2022', 'Maria Alero', '1998-07-15', '+639203614522', 1, 'maria@gmail.com', 'Address Here', 'Branch', 'deposit', '1234567892', 114.00, '1021', 'question1', 'question3', 'Maiden Name', 'Barbie'),
(3, 'mark@123', 'qwerty2021', 'Mark Herras', '1997-06-18', '+639203614523', 1, 'mark@gmail.com', 'Address Here', 'Main Branch', 'deposit', '1234567894', 100.00, '1022', 'question1', 'question3', 'Melissa Alesio', 'GoodFellas'),
(4, 'aris@gmail.com', 'qwerty2021', 'Aris Totle', '1997-06-18', '+639203614525', 1, 'aris@gmail.com', 'Address Here', 'Main Branch', 'deposit', '1234567895', 90.00, '1022', 'question1', 'question3', 'Ariana', 'Goodfellas'),
(5, 'agiela@123', 'qwerty2022', 'Full name', '1998-05-28', '+639203614526', 0, 'agiela@gmail.com', 'Address 123', 'Main Branch', 'deposit', '1234567896', 0.00, '1021', 'question1', 'question3', 'Answer123', 'Answer123'),
(6, 'atom@123', 'qwerty2022', 'Atom Araullo ', '1997-06-12', '+639203614527', 0, 'atomaraullo@gmail.com', 'Address Here', 'Main Branch', 'deposit', '1234567898', 0.00, '1021', 'question1', 'question3', 'Maiden Name', 'movie'),
(7, 'alvin@123', 'qwerty2021', 'Alvin Mariano', '1997-06-20', '+639203614524', 1, 'alvinmariano@gmail.com', 'Address Here', 'Main Branch', 'deposit', '1234567899', 90.00, '1022', 'question1', 'question3', 'Maiden Name', 'Movie');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `moneyrequests`
--
ALTER TABLE `moneyrequests`
  ADD PRIMARY KEY (`RequestId`);

--
-- Indexes for table `transactions`
--
ALTER TABLE `transactions`
  ADD PRIMARY KEY (`TransactionId`);

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `moneyrequests`
--
ALTER TABLE `moneyrequests`
  MODIFY `RequestId` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `transactions`
--
ALTER TABLE `transactions`
  MODIFY `TransactionId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=49;

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
